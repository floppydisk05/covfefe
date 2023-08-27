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

public class HoohCommand : BaseCommandModule {
    [Command("hooh")]
    [Description("H o o h")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task Hooh(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);
        args.scale += 2;

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-hoohDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // H o o h
        MagickImage img = null;
        MagickImageCollection gif = null;
        if (args.extension.ToLower() != "gif") {
            img = new MagickImage(tempImgFile);
            DoHooh(img);
        }
        else {
            gif = new MagickImageCollection(tempImgFile);
            foreach (var frame in gif) DoHooh((MagickImage)frame);
        }

        TempManager.RemoveTempFile(seed + "-hoohDL." + args.extension);
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
        await Context.Channel.SendFileAsync(imgStream, "hooh." + args.extension);
        await msg.DeleteAsync();
    }

    public static void DoHooh(MagickImage image) {
        MagickImage top;
        top = (MagickImage)image.Clone();

        var height = image.Height / 2;
        if (height <= 0)
            height = 1;

        top.Crop(image.Width, height, Gravity.South);
        top.Flip();

        image.Composite(top);
    }
}
