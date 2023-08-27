using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun; 

public class RoryCommand : BaseCommandModule {
    [Command("rory")]
    [Description("Gets a random picture of rory")]
    [Attributes.Category(Category.Fun)]
    public async Task Rory(CommandContext Context) {
        string json;
        // Grab the json string from the API
        using (var client = new WebClient()) {
            json = client.DownloadString("https://rory.cat/purr");
        }

        dynamic output = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

        // Send the image in an embed
        var eb = new DiscordEmbedBuilder();
        eb.WithTitle("Rory");
        eb.WithColor(DiscordColor.Gold);
        eb.WithFooter($"Rory ID: {output.id}");
        eb.WithImageUrl((string)output.url);
        await Context.ReplyAsync("", eb.Build());
    }
}
