using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main; 

public class YearCommand : BaseCommandModule {
    [Command("year")]
    [Description("Gets the remaining time in the year")]
    [Attributes.Category(Category.Fun)]
    public async Task Year(CommandContext Context) {
        var timeSpan = new DateTime(DateTime.Now.Year + 1, 1, 1).Subtract(DateTime.Now);
        await Context.ReplyAsync(
            $"There are {Math.Round(timeSpan.TotalDays, 2)} days left in {DateTime.Now.Year}. That's {Math.Round(timeSpan.TotalHours, 2)} hours, {Math.Round(timeSpan.TotalMinutes, 2)} minutes, or {Math.Round(timeSpan.TotalSeconds, 2)} seconds.");
    }
}
