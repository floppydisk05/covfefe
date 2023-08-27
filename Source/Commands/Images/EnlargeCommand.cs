using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ImageMagick;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Images; 

public class EnlargeCommand : BaseCommandModule {
    [Command("enlarge")]
    [Aliases("e")]
    [Description("Get the raw image of an emote")]
    [Attributes.Category(Category.Images)]
    public async Task Enlarge(CommandContext Context, string emoteStr = null) {
        // Parse the emote string
        if (emoteStr == null)
            throw new Exception("A guild emote to enlarge must be provided!");
        emoteStr = emoteStr.Split(":").LastOrDefault().Replace(">", "");
        ulong.TryParse(emoteStr, out var emoteID);

        // Parse the emote
        DiscordEmoji.TryFromGuildEmote(Bot.client, emoteID, out var emote);
        if (emote == null)
            throw new Exception("The provided emote is invalid! It *must* be a guild emote");

        // Enlarge
        var seed = new Random().Next(1000, 99999);
        var emoteFile = TempManager.GetTempFile($"{seed}-emote.png");
        new WebClient().DownloadFile(emote.Url, emoteFile);
        var image = new MagickImage(emoteFile);
        image.Resize(new MagickGeometry("512x512"));
        image.Write(emoteFile);

        // Send the image
        await Context.Channel.SendFileAsync(emoteFile);
        TempManager.RemoveTempFile($"{seed}-emote.png");
    }
}
