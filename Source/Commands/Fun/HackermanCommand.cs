using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Fun; 

public class HackermanCommand : BaseCommandModule {
    private static readonly string[] things = {
        "non-rotatable disk", "side fumbling CPU", "processor",
        "with multidimension network security access vulnerabilities",
        "oc6 level optical line", "microprocessor architecture", "server", "minecraft server",
        "webserver running Linux 0.01", "Linux system", "shell access terminals", "vulnerable networking firewall",
        "multiphase process memorizer", "x86 IBM level architecture", "network firewall daemon",
        "network routing device",
        "insecure Windows server", "Windows server 1985", "transdimensional phasing device",
        "ultimate security firewalls"
    };

    private static readonly string[] actions = {
        "I've hacked into your ", "I'm breaking into the ", "I've hacked the ",
        "I've gained effective root access to your ",
        "I'm gaining root access to ", "I've hacked into the ", "I broke into the "
    };

    [Command("hackerman")]
    [Description("Hack into the mainframes... just kidding; Hack into Toxidation's network.")]
    [Usage("[length]")]
    [Attributes.Category(Category.Fun)]
    public async Task Hackerman(CommandContext Context, int length = 6) {
        var r = new Random();
        switch (length) {
            case < 6:
                throw new Exception("Length must be greater than or equal to 6");
            case > 100:
                throw new Exception("Length must be less than 100");
        }

        // Generate pure nonsense
        var jargon = actions[r.Next(0, actions.Length)];
        for (var i = 0; i < length; i++) {
            if (i == 1) {
                jargon += "with ";
                continue;
            }

            jargon += things[r.Next(0, things.Length)] + " ";
        }

        // Send an embed
        var eb = new DiscordEmbedBuilder();
        eb.WithColor(DiscordColor.Gold);
        eb.WithDescription($"```cpp\n{jargon.Truncate(2000)}```");
        await Context.ReplyAsync("", eb.Build());
    }
}
