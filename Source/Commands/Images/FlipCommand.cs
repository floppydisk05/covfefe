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

public class FlipCommand : BaseCommandModule {
    [Command("flip")]
    [Description("Flip an image")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task Flip(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-flipDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // F l o p
        MagickImage img = null;
        MagickImageCollection gif = null;
        if (args.extension.ToLower() != "gif") {
            img = new MagickImage(tempImgFile);
            DoFlip(img);
        }
        else {
            gif = new MagickImageCollection(tempImgFile);
            foreach (var frame in gif) DoFlip((MagickImage)frame);
        }

        TempManager.RemoveTempFile(seed + "-flipDL." + args.extension);
        if (args.extension.ToLower() != "gif")
            args.extension = img.Format.ToString().ToLower();

        // Save the image
        var imgStream = new MemoryStream();
        if (args.extension.ToLower() != "gif")
            img.Write(imgStream);
        else
            gif.Write(imgStream);
        imgStream.Position = 0;

        // Send the image
        await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
        await Context.Channel.SendFileAsync(imgStream, "flip." + args.extension);
        await msg.DeleteAsync();
    }

    public static void DoFlip(MagickImage img) {
        img.Flip();
    }
}
