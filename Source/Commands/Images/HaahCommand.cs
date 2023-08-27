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

public class HaahCommand : BaseCommandModule {
    [Command("haah")]
    [Description("H a a h")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task Haah(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);
        args.scale += 2;

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-haahDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // H a a h
        MagickImage img = null;
        MagickImageCollection gif = null;
        if (args.extension.ToLower() != "gif") {
            img = new MagickImage(tempImgFile);
            DoHaah(img);
        }
        else {
            gif = new MagickImageCollection(tempImgFile);
            foreach (var frame in gif) DoHaah((MagickImage)frame);
        }

        TempManager.RemoveTempFile(seed + "-haahDL." + args.extension);
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
        await Context.Channel.SendFileAsync(imgStream, "haah." + args.extension);
        await msg.DeleteAsync();
    }

    public static void DoHaah(MagickImage image) {
        MagickImage right = (MagickImage)image.Clone();

        var width = image.Width / 2;
        if (width <= 0)
            width = 1;

        right.Crop(width, image.Height, Gravity.West);
        right.Rotate(180);
        right.Flip();

        image.Composite(right, width, 0);
    }
}
