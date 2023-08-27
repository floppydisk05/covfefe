using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ImageMagick;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Images; 

public class GaagCommand : BaseCommandModule {
    [Command("gaag")]
    [Description("H")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task Gaag(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);
        args.scale += 2;

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-gaagDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // G a a g. There's probably better ways to do this but meh
        MagickImageCollection gif;
        if (args.extension.ToLower() != "gif")
            return;
        gif = new MagickImageCollection(tempImgFile);
        var tempGif = new MagickImageCollection(tempImgFile);
        tempGif.Reverse();
        foreach (var frame in tempGif) gif.Add(frame);

        TempManager.RemoveTempFile(seed + "-gaagDL." + args.extension);

        // Save the image
        var imgStream = new MemoryStream();
        gif.Write(imgStream);
        imgStream.Position = 0;

        // Send the image
        await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
        await Context.Channel.SendFileAsync(imgStream, "gaag." + args.extension);
        await msg.DeleteAsync();
    }
}
