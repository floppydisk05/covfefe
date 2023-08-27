using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ScottPlot;
using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;

namespace WinBot.Commands.Main; 

public class ServerStatsCommand : BaseCommandModule {
    [Command("serverstats")]
    [Description("Show basic statistics about the server")]
    [Attributes.Category(Category.Main)]
    public async Task Serverstats(CommandContext Context) {
        // Report loading
        var reports = DailyReportSystem.reports.ToList();
        reports.Add(DailyReportSystem.report);
        reports = reports.OrderByDescending(grp => grp.dayOfReport.DayOfYear).Reverse().ToList();

        // Data parsing... this is a huge mess but oh well, I'm lazy and it just works
        // now even MORE of a mess! - Starman Aug 2021

        var realCount = 0;
        for (var i = 0; i < reports.Count; i++)
            if (DateTime.Now.Subtract(reports[i].dayOfReport).TotalDays <= 15)
                realCount++;

        double[] messages = new double[realCount];
        double[] commands = new double[realCount];
        double[] ys = new double[realCount];
        string[] xticks = new string[realCount];
#if TOFU
                userJoin = new double[realCount];
                userLeave = new double[realCount];
#endif

        realCount = 0;
        for (var i = 0; i < reports.Count; i++)
            if (DateTime.Now.Subtract(reports[i].dayOfReport).TotalDays <= 15) {
                ys[realCount] = i;
                xticks[realCount] = reports[i].dayOfReport.ToShortDateString();
                messages[realCount] += reports[i].messagesSent;
                commands[realCount] += reports[i].commandsRan;
#if TOFU
                    userJoin[realCount] += reports[i].usersJoined;
                    userLeave[realCount] += reports[i].usersLeft;
#endif
                realCount++;
            }

        // Plotting
        var plt = new Plot(1920, 1080);
        plt.Style(Color.FromArgb(52, 54, 60), Color.FromArgb(52, 54, 60), null, Color.White, Color.White, Color.White);
        plt.XLabel("Day", null, null, null, 25.5f, false);
        plt.YLabel("Count", null, null, 25.5f, null, false);
        plt.PlotFillAboveBelow(ys, messages, "Messages", lineWidth: 4, lineColor: Color.FromArgb(100, 119, 183),
            fillAlpha: .5, fillColorBelow: Color.FromArgb(100, 119, 183),
            fillColorAbove: Color.FromArgb(100, 119, 183));
        plt.PlotFillAboveBelow(ys, commands, "Command Executions", lineWidth: 4, lineColor: Color.FromArgb(252, 186, 3),
            fillAlpha: .5, fillColorBelow: Color.FromArgb(252, 186, 3), fillColorAbove: Color.FromArgb(252, 186, 3));
#if TOFU
            plt.PlotFillAboveBelow(ys, userJoin, "Users Joined", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(252, 3, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(252, 3, 3), fillColorAbove: System.Drawing.Color.FromArgb(252, 3, 3));
            plt.PlotFillAboveBelow(ys, userLeave, "Users Left", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(15, 252, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(15, 252, 3), fillColorAbove: System.Drawing.Color.FromArgb(15, 252, 3));
#endif
        //plt.TightenLayout(0, true);
        plt.Layout(xScaleHeight: 128);
        plt.XTicks(ys, xticks);
        //plt.Ticks(dateTimeX: true, xTickRotation: 75);
#if !TOFU
        plt.Title($"{Context.Guild.Name} Stats (Past 14 days)", null, null, 45.5f, null, true);
#else
            plt.Title($"{Context.Guild.Name} Stats (UTC, Past 14 days)", null, null, 45.5f, null, true);
#endif
        //plt.Grid(xSpacing: 1, xSpacingDateTimeUnit: ScottPlot.Config.DateTimeUnit.Day);
        plt.Legend(true, null, 30, null, null, Color.FromArgb(100, 52, 54, 60), null, legendLocation.upperRight);

        // Save and send
        var fileName = TempManager.GetTempFile("serverstats.png", true);
        plt.SaveFig(fileName);
        await Context.Channel.SendFileAsync(fileName);
        TempManager.RemoveTempFile("serverstats.png");
    }
}
