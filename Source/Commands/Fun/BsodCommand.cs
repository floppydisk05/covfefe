using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun; 

public class BsodCommand : BaseCommandModule {
    [Command("bsod")]
    [Description("It does the thing™")]
    [Attributes.Category(Category.Fun)]
    public async Task BSOD(CommandContext Context) {
        await Context.ReplyAsync("succ is dead, no succ");
    }
}
