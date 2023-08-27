using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ImageMagick;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Images; 

public class ScrambleCommand : BaseCommandModule {
    [Command("scramble")]
    [Description("H")]
    [Usage("[gif]")]
    [Attributes.Category(Category.Images)]
    public async Task Scramble(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);
        args.scale += 2;

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-randomDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // R a n d o m i z e
        MagickImageCollection gif;
        if (args.extension.ToLower() != "gif")
            return;

        gif = new MagickImageCollection(tempImgFile);
        var tmp = gif.OrderBy(x => new Random().Next()).ToArray();
        gif = new MagickImageCollection(tmp);

        TempManager.RemoveTempFile(seed + "-randomDL." + args.extension);

        // Save the image
        var imgStream = new MemoryStream();
        gif.Write(imgStream);
        imgStream.Position = 0;

        // Send the image
        await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
        await Context.Channel.SendFileAsync(imgStream, "scramble." + args.extension);
        await msg.DeleteAsync();
    }
}
