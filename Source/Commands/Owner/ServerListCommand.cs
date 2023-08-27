using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Owner; 

public class ServerListCommand : BaseCommandModule {
    [Command("serverlist")]
    [Description("Lists guilds the bot is in")]
    [Attributes.Category(Category.Owner)]
    [RequireOwner]
    public async Task ServerList(CommandContext Context) {
        var Embed = new DiscordEmbedBuilder();
        Embed.WithColor(DiscordColor.Gold);
        var output = "";
        foreach (var guild in Bot.client.Guilds) output += $"{guild.Value.Name}\n";
        Embed.WithTitle("Servers");
        Embed.WithDescription(output);
        await Context.ReplyAsync("", Embed.Build());
    }
}
