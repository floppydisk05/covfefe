using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun; 

public class DownloadCommand : BaseCommandModule {
    private string[] nouns;
    private string[] verbs;

    [Command("dl")]
    [Description("You wouldn't download a WinBot!")]
    [Usage("[verb] [noun] [args] (arguments: -noa -red -would -will -inline)")]
    [Attributes.Category(Category.Fun)]
    public async Task Dl(CommandContext Context, string verb = null, [RemainingText] string noun = null) {
        var random = false;
        if (string.IsNullOrWhiteSpace(verb) || string.IsNullOrWhiteSpace(noun)) {
            random = true;

            // Download missing nouns and verbs
            if (!File.Exists("nouns.txt"))
                new WebClient().DownloadFile(
                    "https://raw.githubusercontent.com/aaronbassett/Pass-phrase/master/nouns.txt", "nouns.txt");
            if (!File.Exists("verbs.txt"))
                new WebClient().DownloadFile(
                    "https://raw.githubusercontent.com/aaronbassett/Pass-phrase/master/verbs.txt", "verbs.txt");

            // Fill nouns and verbs if they're empty
            if (nouns == null)
                nouns = File.ReadAllLines("nouns.txt");
            if (verbs == null)
                verbs = File.ReadAllLines("verbs.txt");

            verb = verbs[new Random().Next(verbs.Length)];
            noun = nouns[new Random().Next(nouns.Length)];
        }

        if ((verb.ToLower() == "rick" && noun.ToLower().Contains("roll")) ||
            (verb.ToLower().Contains("rick") && verb.ToLower().Contains("roll"))) {
            await Context.Channel.SendFileAsync("Resources/rick.gif");
            return;
        }

        // Convoluted options implemented as poorly as possible
        var noA = false;
        var red = false;
        var would = false;
        var will = false;
        var inline = false;
        if (noun.Contains("-noa") || noun.EndsWith("s")) noA = true;
        if (noun.Contains("-red")) red = true;
        if (noun.Contains("-would")) would = true;
        if (noun.Contains("-will")) will = true;
        if (noun.Contains("-inline")) inline = true;
        noun = noun.Replace("-noa", "").Replace("-red", "").Replace("-would", "").Replace("-will", "")
            .Replace("-inline", "");

        if (random) {
            would = new Random().Next(0, 100) > 75;
            if (!would)
                will = new Random().Next(0, 100) > 95;
        }

        // Shit I shouldn't have to do
        var fonts = new PrivateFontCollection();
        fonts.AddFontFile("Resources/xband.ttf");

        // Create the image
        var img = new Bitmap(1280, 720);
        var bmp = Graphics.FromImage(img);
        bmp.Clear(Color.Black);

        // Set up the fonts and drawing stuff
        var YOUWOULDNTDOWNLOADACARfont = new Font(
            fonts.Families[0].Name,
            175,
            FontStyle.Regular,
            GraphicsUnit.Pixel
        );
        SolidBrush brush = !red ? new SolidBrush(Color.White) : new SolidBrush(Color.FromArgb(102, 0, 0));

        // My best friend, RNG
        float youWouldnt = new Random().Next(-100, 100);
        float verbX = 135 + new Random().Next(25, 220);
        var nounX = 242.32f + new Random().Next(25, 120);
        if (inline) {
            youWouldnt = -100;
            verbX = 65.05f;
            nounX = 65.05f;
        }

        // Draw the text onto the image
        bmp.DrawString("you", YOUWOULDNTDOWNLOADACARfont, brush, 165.05f + youWouldnt, 75.0f);
        if (!would && !will) bmp.DrawString("wouldn't", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
        else if (!will) bmp.DrawString("would", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
        else bmp.DrawString("will", YOUWOULDNTDOWNLOADACARfont, brush, 465.0f + youWouldnt, 125.0f);
        bmp.DrawString(verb + (noA ? "" : " a"), YOUWOULDNTDOWNLOADACARfont, brush, verbX, 325.5f);
        //if (!noA) { 
        //    SizeF aOff = MiscUtil.MeasureString("a", YOUWOULDNTDOWNLOADACARfont);
        //    bmp.DrawString("a", YOUWOULDNTDOWNLOADACARfont, brush, verbX + (verb.Length * 95), 300.5f);
        //}
        bmp.DrawString(noun, YOUWOULDNTDOWNLOADACARfont, brush, nounX, 500.3f);

        // Save the image to a temporary file, this has to be two separate Save functions because apparently that's what Microsoft thinks is good
        bmp.Save();
        img.Save("Resources/download.png");

        await Context.Channel.SendFileAsync("Resources/download.png");
    }
}
