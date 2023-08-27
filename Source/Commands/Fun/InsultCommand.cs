using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.fun; 

public class InsultCommand : BaseCommandModule {
    [Command("insult")]
    [Description("Insults you. Fun.")]
    [Attributes.Category(Category.Fun)]
    public async Task Insult(CommandContext Context) {
        var eb = new DiscordEmbedBuilder();
        // Get the thing
        string json;
        using (var http = new HttpClient()) {
            json = await http.GetStringAsync("http://evilinsult.com/generate_insult.php?lang=en&type=json");
        }

        dynamic insultText = JsonConvert.DeserializeObject(json); // Deserialize the thang
        // Send the thing
        eb.WithTitle("Get insulted... loser.");
        eb.WithColor(DiscordColor.Gold);
        eb.WithDescription($"{insultText.insult}");

        await Context.ReplyAsync("", eb.Build());
    }
}
