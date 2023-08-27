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

public class TVCommand : BaseCommandModule {
    private static readonly string[] images = { "celebrate", "remote", "normal", "angry" };

    [Command("tv")]
    [Description("Watch TV or something idk")]
    [Usage("[image]")]
    [Attributes.Category(Category.Images)]
    public async Task TV(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-tvDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        MagickImage img = null;
        MagickImageCollection gif = null;
        if (args.extension.ToLower() != "gif") {
            img = new MagickImage(tempImgFile);
            img = DoTV(img, args);
        }
        else {
            gif = new MagickImageCollection(tempImgFile);
            foreach (var frame in gif) {
                DoTV((MagickImage)frame, args, true);
                frame.Resize(400, 300);
            }
        }

        TempManager.RemoveTempFile(seed + "-tvDL." + args.extension);

        // Save the image
        var imgStream = new MemoryStream();
        if (args.extension.ToLower() != "gif")
            img.Write(imgStream);
        else
            gif.Write(imgStream);
        imgStream.Position = 0;

        // Send the image
        await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
        await Context.Channel.SendFileAsync(imgStream, "tv." + args.extension);
        await msg.DeleteAsync();
    }

    private MagickImage DoTV(MagickImage img, ImageArgs args, bool isGif = false) {
        // Composite args
        var rotation = 0.15f;
        var srcX = 260;
        var srcY = 145;
        var compX = 166;
        var compY = 45;
        var imageFile = "tv.png";

        // Setup
        if (string.IsNullOrWhiteSpace(args.textArg))
            args.textArg = images[new Random().Next(0, images.Length)];

        if (args.textArg.ToLower() == "celebrate") {
            compX = 196;
            compY = 64;
            srcX = 149;
            srcY = 84;
            rotation = 0;
            imageFile = "tv2.png";
        }
        else if (args.textArg.ToLower() == "remote") {
            compX = 95;
            compY = 35;
            srcX = 459;
            srcY = 276;
            rotation = 0;
            imageFile = "tv3.png";
        }
        else if (args.textArg.ToLower() == "angry") {
            compX = 75;
            compY = 145;
            srcX = 280;
            srcY = 165;
            rotation = 0;
            imageFile = "tv4.png";
        }

        var tv = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));
        var tvClean = new MagickImage(ResourceManager.GetResourcePath(imageFile, ResourceType.Resource));

        // Composite
        img.Resize(new MagickGeometry($"{srcX}x{srcY}!"));
        img.BackgroundColor = MagickColors.Transparent;
        img.Rotate(rotation);
        tv.Alpha(AlphaOption.Remove);
        tv.Composite(img, compX, compY, CompositeOperator.SrcIn);
        if (args.textArg.ToLower() == "remote" || args.textArg.ToLower() == "angry")
            tv.Composite(tvClean, 0, 0, CompositeOperator.SrcOver, "-background none");
        if (isGif) {
            img.Resize(new MagickGeometry($"{tv.Width}x{tv.Height}!"));
            img.Rotate(rotation * -1);
            img.CopyPixels(tv);
            return null;
        }

        return tv;
    }
}
