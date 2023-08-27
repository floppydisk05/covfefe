using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;
using WinBot.Misc;

namespace WinBot.Commands.Main; 

public class LevelMessageCommand : BaseCommandModule {
    [Command("togglelevelmsg")]
    [Description("Toggle level messages")]
    [Attributes.Category(Category.Main)]
    public async Task LevelMessage(CommandContext Context) {
        var user = UserData.GetOrCreateUser(Context.User);
        user.levelMessages = !user.levelMessages;
        var state = user.levelMessages ? "on" : "off";
        await Context.ReplyAsync($"Your level messages are now {state}!");
    }
}
