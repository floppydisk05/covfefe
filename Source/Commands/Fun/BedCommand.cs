using System;
using System.Drawing;
using System.Drawing.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;
using WinBot.Util;
using static WinBot.Util.ResourceManager;

namespace WinBot.Commands.Fun; 

public class BedCommand : BaseCommandModule {
    private static readonly string[] images = { "NONE", "mehdi", "agp", "agp2" };

    [Command("bed")]
    [Description("Tell someone to go to bed.")]
    [Usage("[User] [Image: parz, agp, agp2, mehdi]")]
    [Attributes.Category(Category.Fun)]
    public async Task bed(CommandContext Context, string screenname = "", string image = "parz") {
        // Randomize the image if no input was given
        if (image == "parz" || string.IsNullOrWhiteSpace(image))
            image = images[new Random().Next(0, images.Length)];

        // Select the image with some YanDev code
        var imageFile = GetResourcePath("parz.png", ResourceType.Resource);
        var genX = 365f;
        var bedY = 506f;
        var userY = 615f;
        var fontSize = 70;
        switch (image.ToLower()) {
            case "agp":
                imageFile = GetResourcePath("agp.png", ResourceType.Resource);
                genX = 125;
                bedY = 35;
                fontSize = 50;
                userY = 360;
                break;
            
            case "agp2":
                imageFile = GetResourcePath("agp2.png", ResourceType.Resource);
                genX = 177.5f;
                bedY = 60;
                fontSize = 50;
                userY = 450;
                break;
            
            case "mehdi":
                imageFile = GetResourcePath("mehdi.png", ResourceType.Resource);
                bedY = 100;
                genX = 307.5f;
                userY = 680f;
                break;
        }
        
        // Load the font
        var fonts = new PrivateFontCollection();
        fonts.AddFontFile(GetResourcePath("impact.ttf", ResourceType.Resource));

        // Create the image
        var img = new Bitmap(Image.FromFile(imageFile));
        var bmp = Graphics.FromImage(img);

        // Set up the fonts and drawing stuff
        var IMPACTfont = new Font(
            fonts.Families[0].Name,
            fontSize,
            FontStyle.Regular,
            GraphicsUnit.Pixel
        );

        SolidBrush brush = new SolidBrush(Color.White);
        var snFormat = new StringFormat();
        snFormat.Alignment = StringAlignment.Center;
        snFormat.LineAlignment = StringAlignment.Center;

        // Draw the text onto the image
        bmp.DrawString("GO TO BED", IMPACTfont, brush, genX, bedY, snFormat);
        bmp.DrawString(screenname.ToUpper(), IMPACTfont, brush, genX, userY, snFormat);

        // Save the image to a temporary file
        bmp.Save();
        var imagePath = TempManager.GetTempFile($"bed-{image}-" + DateTime.Now.Ticks + "-{Context.User.Id}.png", true);
        img.Save(imagePath);
        await Context.Channel.SendFileAsync(imagePath);
        TempManager.RemoveTempFile($"bed-{image}-" + DateTime.Now.Ticks + "-{Context.User.Id}.png");
    }
}
