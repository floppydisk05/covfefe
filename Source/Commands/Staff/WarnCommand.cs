using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main; 

public class WarnCommand : BaseCommandModule {
    [Command("warn")]
    [Description("Warn a user")]
    [Usage("[user] [reason]")]
    [Attributes.Category(Category.Staff)]
    [RequireUserPermissions(Permissions.KickMembers)]
    public async Task Warn(CommandContext Context, DiscordMember user, [RemainingText] string reason = null) {
        if (reason == null)
            reason = "No reason given"; // Default reason

        var warnEmbed = new DiscordEmbedBuilder();
        var now = DateTime.Now;
        var date = $"{now.Day}/{now.Month}/{now.Year}";
        warnEmbed.WithTitle($":warning: {Context.Guild.Name} · Warn");
        warnEmbed.WithDescription(
            $"**{Context.Message.Author.Username}#{Context.Message.Author.Discriminator} warned you for the following reason:**```{reason}```");
        warnEmbed.WithFooter($"Punishment · {date}", Context.Guild.IconUrl);
        warnEmbed.WithColor(new DiscordColor("#ffff00"));

        // Send a message in the current channel and an embed to the user
        await user.SendMessageAsync(warnEmbed);
        await Context.RespondAsync($"`{user.Username}#{user.Discriminator}` has been warned for \"{reason}\"");
    }
}
