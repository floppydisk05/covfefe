using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Main; 

public class TranslateCommand : BaseCommandModule {
    [Command("tr")]
    [Description("Translates text from one language to another")]
    [Usage("[-from=code] [-to=code] [text]")]
    [Attributes.Category(Category.Main)]
    public async Task Translate(CommandContext Context, [RemainingText] string text = null) {
        var source = "";
        var target = "";
        var arg1 = "";
        var arg2 = "";
        const string apiUrl = "https://translate.argosopentech.com/";
        var confidence = "";
        //bool auto = false;
        var client = new HttpClient();
        try {
            arg1 = text.Split()[0];
        }
        catch {
            arg1 = "";
        }

        try {
            arg2 = text.Split()[1];
        }
        catch {
            arg2 = "";
        }

        if (arg1.Contains("-from="))
            source = arg1.Replace("-from=", "");
        else if (arg1.Contains("-to=")) target = arg1.Replace("-to=", "");

        if (arg2.Contains("-from="))
            source = arg2.Replace("-from=", "");
        else if (arg2.Contains("-to=")) target = arg2.Replace("-to=", "");
        var sourceText = text.Replace($"-from={source}", "");
        sourceText = sourceText.Replace($"-to={target}", "");
        if (source == "") {
            var values2 = new Dictionary<string, string> {
                { "q", sourceText },
                { "api_key", "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" }
            };
            var data2 = new FormUrlEncodedContent(values2);
            var response2 = await client.PostAsync($"{apiUrl}detect", data2);
            dynamic responseData2 = JsonConvert.DeserializeObject(await response2.Content.ReadAsStringAsync());
            source = responseData2[0].language;
            //auto = true;
            confidence = $"{responseData2[0].confidence}";
        }

        if (target == "") target = "en";


        var values = new Dictionary<string, string> {
            { "q", sourceText },
            { "source", source },
            { "target", target },
            { "format", "text" },
            { "api_key", "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" }
        };

        //string apiUrl = "http://nick99nack.com:5000/translate";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync($"{apiUrl}translate", data);
        dynamic responseData = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        string translatedText = Convert.ToString(responseData.translatedText);
        if (translatedText.Length >= 1024) {
            const string baseUrl = "http://paste.nick99nack.com/";
            var hasteBinClient = new HasteBinClient(baseUrl);
            var HBresult = hasteBinClient.Post(translatedText).Result;
            translatedText = $"{baseUrl}{HBresult.Key}";
        }

        var eb = new DiscordEmbedBuilder();
        eb.WithTitle("Translate");
        eb.AddField("Translation Pathway", $"{source} -> {target}");
        /*eb.AddField("Original", sourceText, true);*/
        eb.AddField("Translated", translatedText.Truncate(1024), true);
        /*if (auto) eb.WithFooter($"Source Confidence {confidence}\nProvided by LibreTranslate");
        else eb.WithFooter("Provided by LibreTranslate");*/
        eb.WithFooter($"Source Confidence {confidence}%");
        eb.WithColor(DiscordColor.Gold);

        await Context.ReplyAsync("", eb.Build());
    }
}
