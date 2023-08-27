using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Fun; 

public class FortuneCommand : BaseCommandModule {
    [Command("fortune")]
    [Description("It's the BSD fortune program")]
    [Usage("fortune")]
    [Attributes.Category(Category.Fun)]
    public async Task Exec(CommandContext Context) {
        const string command = "/usr/games/fortune -a";
        var eb = new DiscordEmbedBuilder();
        eb.WithTitle("Fortune");
        eb.WithColor(DiscordColor.Gold);
        eb.WithDescription($"```{command.Bash()}```");
        await Context.ReplyAsync("", eb.Build());
    }
}
