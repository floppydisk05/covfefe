using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main; 

public class AboutCommand : BaseCommandModule {
    [Command("about")]
    [Description("Gets basic info about the bot")]
    [Attributes.Category(Category.Main)]
    public async Task About(CommandContext Context) {
        var eb = new DiscordEmbedBuilder();
        eb.WithTitle($"{Bot.client.CurrentUser.Username}");
        eb.AddField("Maintainer", $"{Bot.client.CurrentApplication.Owners.First().Username}", true);
        eb.AddField("Contributors", "floppydisk\nxproot\nHIDEN", true);
        eb.AddField("Language", "C#", true);
        eb.AddField("Version", Bot.VERSION, true);
        eb.AddField("Library", $"DSharpPlus v{Bot.client.VersionString}", true);
        eb.AddField("Host OS", Environment.OSVersion.VersionString, true);
        eb.WithUrl("https://github.com/floppyNET/covfefe");
        eb.WithThumbnail(Bot.client.CurrentUser.AvatarUrl);
        eb.WithColor(DiscordColor.Gold);

        await Context.ReplyAsync("", eb.Build());
    }
}
