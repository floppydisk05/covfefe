using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main; 

public class PollCommand : BaseCommandModule {
    private static readonly DiscordEmoji[] optionEmotes = {
        DiscordEmoji.FromUnicode("🇦"), DiscordEmoji.FromUnicode("🇧"),
        DiscordEmoji.FromUnicode("🇨"), DiscordEmoji.FromUnicode("🇩"),
        DiscordEmoji.FromUnicode("🇪"), DiscordEmoji.FromUnicode("🇫"),
        DiscordEmoji.FromUnicode("🇬"), DiscordEmoji.FromUnicode("🇭"),
        DiscordEmoji.FromUnicode("🇮"), DiscordEmoji.FromUnicode("🇯")
    };

    [Command("poll")]
    [Description("Create a poll")]
    [Usage("[title] ['|' separated options]")]
    [Attributes.Category(Category.Staff)]
    [RequireUserPermissions(Permissions.KickMembers)]
    public async Task Poll(CommandContext Context, string title, [RemainingText] string optionString) {
        // Null checks
        if (string.IsNullOrWhiteSpace(title))
            throw new Exception("You must provide a title!");
        if (string.IsNullOrWhiteSpace(optionString))
            throw new Exception("You must provide options!");

        // Parse/verify options
        var options = optionString.Split('|');
        switch (options.Length) {
            case < 2:
                throw new Exception("You must provide *at least* two options.");
            case > 10:
                throw new Exception("You cannot have more than 10 options!");
        }

        for (var i = 0; i < options.Length; i++)
            options[i] = $"{optionEmotes[i]} {options[i]}";

        // Create an embed
        var eb = new DiscordEmbedBuilder();
        eb.WithTitle(title);
        eb.WithColor(DiscordColor.Gold);
        eb.WithTimestamp(DateTime.Now);
        eb.WithFooter($"Poll started by {Context.User.Username}#{Context.User.Discriminator}");
        eb.AddField("Options", string.Join('\n', options));
        var msg = await Context.ReplyAsync("", eb.Build());

        // Add reactions
        for (var i = 0; i < options.Length; i++) {
            await msg.CreateReactionAsync(optionEmotes[i]);
            await Task.Delay(512);
        }
    }
}
