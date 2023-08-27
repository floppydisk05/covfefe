using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Owner; 

public class HostStatusCommand : BaseCommandModule {
    [Command("hoststatus")]
    [Description("Gets info about the host machine")]
    [Aliases("host", "hostinfo")]
    [Attributes.Category(Category.Owner)]
    [RequireOwner]
    public async Task HostStatus(CommandContext Context) {
        var Embed = new DiscordEmbedBuilder();
        Embed.WithColor(DiscordColor.Gold);
        Embed.WithDescription(
            $"```{ParseNF("neofetch --disable title resolution theme icons gpu term --stdout".Bash().Replace(" \nOS:", "OS:"))}```");
        await Context.ReplyAsync("", Embed.Build());
    }

    public string ParseNF(string neofetch) {
        var parsed = neofetch;
        parsed = parsed
            .Replace("OS: ", "OS:            ")
            .Replace("Host: ", "Host           ")
            .Replace("Kernel: ", "Kernel:        ")
            .Replace("Uptime: ", "Uptime:        ")
            .Replace("Packages: ", "Packages:      ")
            .Replace("Shell: ", "Shell:         ")
            .Replace("DE: ", "DE:            ")
            .Replace("WM: ", "WM:            ")
            .Replace("CPU: ", "CPU:           ")
            .Replace("GPU: ", "GPU:           ")
            .Replace("Memory: ", "Memory:        ");
        return parsed;
    }
}
