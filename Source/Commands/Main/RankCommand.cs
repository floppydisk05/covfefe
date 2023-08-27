using System.Drawing;
using System.Drawing.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;
using static WinBot.Util.ResourceManager;

namespace WinBot.Commands.Main; 

public class RankCommand : BaseCommandModule {
    [Command("rank")]
    [Description("Get your current level")]
    [Usage("[user]")]
    [Attributes.Category(Category.Main)]
    public async Task Rank(CommandContext Context, [RemainingText] DiscordUser dUser = null) {
        if (dUser == null)
            dUser = Context.User;

        // Get info
        var user = UserData.GetOrCreateUser(dUser);
        float neededXP = (user.level + 1) * 5 * 40;

        // Fetch the user avater
        var url = dUser.GetAvatarUrl(ImageFormat.Jpeg, 256);
        var avatarStream = await new HttpClient().GetStreamAsync(url);
        var avatar = new Bitmap(Image.FromStream(avatarStream), new Size(230, 230));
        avatarStream.Close();

        // Set up text drawing
        var fonts = new PrivateFontCollection();
        fonts.AddFontFile(GetResourcePath("Roboto-Regular.ttf", ResourceType.Resource));
        var brush = new SolidBrush(Color.White);
        var drawForm = new StringFormat();
        var roboto = new Font(fonts.Families[0].Name, 50, FontStyle.Regular, GraphicsUnit.Pixel);
        var robotoSmall = new Font(fonts.Families[0].Name, 35, FontStyle.Regular, GraphicsUnit.Pixel);

        // Calculate offsets
        var usernameSize = MiscUtil.MeasureString(Regex.Replace(dUser.Username, @"\p{Cs}", ""), roboto);
        var levelSize = MiscUtil.MeasureString("LEVEL", robotoSmall);
        var levelNumSize = MiscUtil.MeasureString($"{user.level}", roboto);
        var xpSize =
            MiscUtil.MeasureString($"{MiscUtil.FormatNumber((int)user.xp)}/{MiscUtil.FormatNumber((int)neededXP)}",
                robotoSmall);
        var progressBar = (int)(user.xp / neededXP * 616);

        // Image creation:

        // Setup
        var bmp = new Bitmap(934, 282);
        var img = Graphics.FromImage(bmp);
        img.Clear(Color.FromArgb(35, 39, 42));

        // Graphics
        img.DrawImage(avatar,
            new Point(26, 26)); // 26 in on x and y with a 230x230 avatar allows for all 3 sides to be evenly spaced.
        brush.Color = Color.FromArgb(72, 75, 78);
        img.FillPath(brush, MiscUtil.RoundedRect(new Rectangle(292, 200, 616, 50), 24));
        brush.Color = MiscUtil.GetAverageColor(avatar);
        if (progressBar >= 40)
            img.FillPath(brush, MiscUtil.RoundedRect(new Rectangle(292, 200, progressBar, 50), 24));

        // Strings
        brush.Color = Color.White;
        img.DrawString(Regex.Replace(dUser.Username, @"\p{Cs}", ""), roboto, brush, new PointF(292, 140));
        img.DrawString($"{MiscUtil.FormatNumber((int)user.xp)}/{MiscUtil.FormatNumber((int)neededXP)}", robotoSmall,
            brush, new Point(600 - (int)xpSize.Width / 2, 205));
        brush.Color = MiscUtil.GetAverageColor(avatar);
        img.DrawString("LEVEL", robotoSmall, brush,
            new Point(908 - (int)levelNumSize.Width - 6 - (int)levelSize.Width, 26));
        img.DrawString($"{user.level}", roboto, brush, new Point(908 - (int)levelNumSize.Width, 16));
        brush.Color = Color.Gray;
        img.DrawString($"#{dUser.Discriminator}", robotoSmall, brush, new PointF(292 + usernameSize.Width, 150));

        // Save and send the image
        var imagePath = TempManager.GetTempFile($"rankCard-{user.username}-{user.id}.png", true);
        bmp.Save(imagePath);
        await Context.Channel.SendFileAsync(imagePath);
        TempManager.RemoveTempFile($"rankCard-{user.username}-{user.id}.png");
    }
}
