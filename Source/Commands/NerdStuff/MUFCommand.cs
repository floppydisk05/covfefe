using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ImageMagick;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.NerdStuff; 

public class MUFCommand : BaseCommandModule {
    [Command("muf")]
    [Description("Sends a map of radio MUF")]
    [Attributes.Category(Category.NerdStuff)]
    public async Task MUF(CommandContext Context) {
        // This code is garbage and barely works... but it does work.
        var svgFile = TempManager.GetTempFile("muf.svg");
        var pngOut = TempManager.GetTempFile("muf2.png", true);
        new WebClient().DownloadFile("https://prop.kc2g.com/renders/current/mufd-normal-now.svg", svgFile);
        var img = new MagickImage(svgFile);
        img.Format = MagickFormat.Png;
        img.Write(pngOut);
        await Context.Channel.SendFileAsync(pngOut);
    }
}
