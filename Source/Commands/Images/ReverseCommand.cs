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

public class ReverseCommand : BaseCommandModule {
    [Command("reverse")]
    [Description("Reverse a gif, because why not?")]
    [Usage("[gif]")]
    [Attributes.Category(Category.Images)]
    public async Task Reverse(CommandContext Context, [RemainingText] string input) {
        // Handle arguments
        var args = ImageCommandParser.ParseArgs(Context, input);
        var seed = new Random().Next(1000, 99999);

        if (args.extension.ToLower() != "gif")
            throw new Exception("Image provided is not a gif!");

        // Download the image
        var tempImgFile = TempManager.GetTempFile(seed + "-reverseDL." + args.extension, true);
        new WebClient().DownloadFile(args.url, tempImgFile);

        var msg = await Context.ReplyAsync("Processing...\nThis may take a while depending on the image size");

        // Add s p e e d
        var gif = new MagickImageCollection(tempImgFile);
        gif.Reverse();
        TempManager.RemoveTempFile(seed + "-reverseDL." + args.extension);

        // Save the image
        var imgStream = new MemoryStream();
        gif.Write(imgStream);
        imgStream.Position = 0;

        // Send the image
        await msg.ModifyAsync("Uploading...\nThis may take a while depending on the image size");
        await Context.Channel.SendFileAsync(imgStream, "reverse." + args.extension);
        await msg.DeleteAsync();
    }
}
