using System;
using System.IO;
using System.Timers;
using Genbox.Wikipedia.Objects;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace WinBot.Util; 

public class DiscordSink : ILogEventSink {
    private readonly IFormatProvider _formatProvider;
    private string logBuffer = "";

    public DiscordSink(IFormatProvider formatProvider) {
        _formatProvider = formatProvider;

        var t = new Timer(5000);
        t.AutoReset = true;
        t.Elapsed += async (s, e) => {
            if (!string.IsNullOrWhiteSpace(logBuffer) && Global.logChannel != null) {
                await Global.logChannel.SendMessageAsync(logBuffer);
                logBuffer = "";
            }
        };
        t.Start();
    }

    public void Emit(LogEvent logEvent) {
        var message = logEvent.RenderMessage(_formatProvider);
        var finalMessage =
            $"[{DateTime.Now.ToShortDateString().Replace("/", "-")} {DateTime.Now.ToShortTimeString()} {logEvent.Level.ToString()}] " +
            message;

        // Change console colour
        switch (logEvent.Level) {
            case LogEventLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogEventLevel.Fatal:
            case LogEventLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogEventLevel.Debug:
            case LogEventLevel.Verbose:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }

        // Print and write the log message
        Console.WriteLine(finalMessage);
        File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}.log", finalMessage + "\n");
        Console.ResetColor();

        // Store the log in the Discord buffer
        logBuffer += finalMessage + "\n";
    }
}

public static class DiscordSinkExtensions {
    public static LoggerConfiguration DiscordSink(this LoggerSinkConfiguration loggerSinkConfiguration,
        IFormatProvider formatProvider = null) {
        return loggerSinkConfiguration.Sink(new DiscordSink(formatProvider));
    }
}
