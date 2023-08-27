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

public class InvertCommand : BaseCommandModule {
    [Command("invert")]
    [Description("Invert an image")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task Invert(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-invertDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // I n v e r t
        MagickImage img = null;
        MagickImageCollection gif = null;
        if (args.extension.ToLower() != "gif") {
            img = new MagickImage(tempImgFile);
            DoInvert(img);
        }
        else {
            gif = new MagickImageCollection(tempImgFile);
            foreach (var frame in gif) DoInvert((MagickImage)frame);
        }

        TempManager.RemoveTempFile(seed + "-invertDL." + args.extension);
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
        await Context.Channel.SendFileAsync(imgStream, "invert." + args.extension);
        await msg.DeleteAsync();
    }

    public static void DoInvert(MagickImage img) {
        img.Negate(Channels.RGB);
    }
}
