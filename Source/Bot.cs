using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using ImageMagick;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using WinBot.Commands;
using WinBot.Misc;
using WinBot.Util;

namespace WinBot;

using static ResourceManager;

internal class Bot {
    public const string VERSION = "4.1.0";

    // DSharpPlus
    public static DiscordClient client;
    public static CommandsNextExtension commands;
    public static Stopwatch sw = Stopwatch.StartNew();

    // Bot
    public static BotConfig config;

    public static void Main(string[] args) {
        new Bot().RunBot().GetAwaiter().GetResult();
    }

    public async Task RunBot() {
        // Change workingdir in debug mode
#if DEBUG
        if (!Directory.Exists("WorkingDirectory"))
            Directory.CreateDirectory("WorkingDirectory");
        Directory.SetCurrentDirectory("WorkingDirectory");
#endif

        // Logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.DiscordSink()
            .CreateLogger();
        var logFactory = new LoggerFactory().AddSerilog();
        Log.Information($"WinBot {VERSION}");
        Log.Information("Starting bot...");

        VerifyIntegrity();
        LoadConfigs();

        // Set up the Discord client
        client = new DiscordClient(new DiscordConfiguration {
            Token = config.token,
            TokenType = TokenType.Bot,
            LoggerFactory = logFactory,
            Intents = DiscordIntents.All
        });
        commands = client.UseCommandsNext(new CommandsNextConfiguration {
            StringPrefixes = new[] { config.prefix },
            EnableDefaultHelp = false,
            EnableDms = true,
            UseDefaultCommandHandler = false
        });
        client.UseInteractivity(new InteractivityConfiguration {
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(60)
        });
        HookEvents();
        commands.RegisterCommands(Assembly.GetExecutingAssembly());
        await client.ConnectAsync();

        await Task.Delay(-1);
    }

    private static async Task Ready(DiscordClient client, ReadyEventArgs e) {
        // Set guilds
        Global.hostGuild = await client.GetGuildAsync(config.ids.hostGuild);
        Global.targetGuild = await client.GetGuildAsync(config.ids.targetGuild);

        // Set channels
        if (config.ids.logChannel != 0)
            Global.logChannel = await client.GetChannelAsync(config.ids.logChannel);
        if (Global.logChannel == null)
            Log.Error("Shitcord is failing to return a valid log channel or no channel ID is set in the config");
        
        // Start misc systems
        UserData.Init();
        Leveling.Init();
        TempManager.Init();
        DailyReportSystem.Init();
        MagickNET.Initialize();

        await client.UpdateStatusAsync(new DiscordActivity { Name = config.status });
        Log.Information("Ready");
    }

    private void HookEvents() {
        // Bot
        client.Ready += Ready;
        client.MessageCreated += CommandHandler.HandleMessage;
        client.MessageUpdated += (client, e) => {
            if (DateTime.Now.Subtract(e.Message.Timestamp.DateTime).TotalMinutes < 1 &&
                DateTime.Now.Subtract(e.Message.Timestamp.DateTime).TotalSeconds > 2)
                CommandHandler.HandleCommand(e.Message, e.Author);
            return Task.CompletedTask;
        };
        client.GuildMemberAdded += async (client, e) => {
            if (Global.mutedUsers.Contains(e.Member.Id))
                await e.Member.GrantRoleAsync(Global.mutedRole, "smh what an idiot");
        };
        // Commands
        commands.CommandErrored += CommandHandler.HandleError;

        EventLogging.Init();
    }

    private static void VerifyIntegrity() {
        Log.Write(LogEventLevel.Information, "Verifying integrity of bot files...");

        // Verify directories
        if (!Directory.Exists("Logs"))
            Directory.CreateDirectory("Logs");
        if (!Directory.Exists("Data"))
            Directory.CreateDirectory("Data");
        if (!Directory.Exists("Resources"))
            Directory.CreateDirectory("Resources");
        // Extrememly awful way to do this, but I guess it'll work for now - HIDEN
        if (!Directory.Exists("Resources/Lyrics"))
            Directory.CreateDirectory("Resources/Lyrics");
        if (!Directory.Exists("Temp"))
            Directory.CreateDirectory("Temp");

        // Verify configs & similar files
        if (!ResourceExists("config", ResourceType.Config)) {
            // Create a blank config
            config = new BotConfig();
            config.token = "TOKEN";
            config.status = " ";
            config.prefix = ".";
            config.ids = new IDConfig();
            config.apiKeys = new APIConfig();
            config.minecraftServers = new MCServer[] { new() };

            // Write the config and quit
            File.WriteAllText(GetResourcePath("config", ResourceType.Config),
                JsonConvert.SerializeObject(config, Formatting.Indented));
            Log.Fatal("No configuration file found. A template config has been written to config.json");
            Environment.Exit(-1);
        }

        if (!ResourceExists("blacklist", ResourceType.JsonData))
            File.WriteAllText(GetResourcePath("blacklist", ResourceType.JsonData), "[]");
        if (!ResourceExists("mute", ResourceType.JsonData))
            File.WriteAllText(GetResourcePath("mute", ResourceType.JsonData), "[]");

        // Verify and download resources
        Log.Information("Verifying resources...");
        var webClient = new WebClient();
        var resourcesJson =
            webClient.DownloadString(
                "https://raw.githubusercontent.com/floppyNET/covfefe/master/Resources/resources.json");
        var resources = JsonConvert.DeserializeObject<string[]>(resourcesJson);
        foreach (var resource in resources)
            if (!ResourceExists(resource, ResourceType.Resource)) {
                webClient.DownloadFile(
                    $"https://raw.githubusercontent.com/floppyNET/covfefe/master/Resources/{resource}",
                    GetResourcePath(resource, ResourceType.Resource));
                Log.Information("Downloaded " + resource + "");
            }

        // This is awful awful awful awful awful AWFUL to do this on every startup
        // but I'm lazy and it's the only way I can think of right now to make the bot
        // update lyrics on startup lol
        foreach (var file in Directory.GetFiles("Resources/Lyrics"))
            File.Delete(file);
        ZipFile.ExtractToDirectory("Resources/Lyrics.zip", "Resources/");
    }

    private static void LoadConfigs() {
        // Main bot config
        config = JsonConvert.DeserializeObject<BotConfig>(
            File.ReadAllText(GetResourcePath("config", ResourceType.Config)));
        if (config == null) {
            Log.Fatal("Failed to load configuration!");
            Environment.Exit(-1);
        }

        Global.blacklistedUsers =
            JsonConvert.DeserializeObject<List<ulong>>(
                File.ReadAllText(GetResourcePath("blacklist", ResourceType.JsonData)));
        Global.mutedUsers =
            JsonConvert.DeserializeObject<List<ulong>>(
                File.ReadAllText(GetResourcePath("mute", ResourceType.JsonData)));
    }
}

internal class BotConfig {
    public string token { get; set; }
    public string prefix { get; set; }
    public string status { get; set; }
    public IDConfig ids { get; set; }
    public APIConfig apiKeys { get; set; }
    public MCServer[] minecraftServers { get; set; }
}

internal class IDConfig {
    public ulong hostGuild { get; set; } = 0; // Where logs etc are
    public ulong targetGuild { get; set; } = 0; // Where muted role etc are
    public ulong logChannel { get; set; } = 0;
    public ulong mutedRole { get; set; } = 0;
}

internal class APIConfig {
    public string wikihowAPIKey { get; set; } = "";
    public string catAPIKey { get; set; } = "";
    public string weatherAPI { get; set; } = "";
}

internal class Global {
    public static List<List<string>> reminders = new();
    public static DiscordGuild hostGuild;
    public static DiscordGuild targetGuild;
    public static DiscordChannel logChannel;

    // Moderation
    public static List<ulong> blacklistedUsers = new();
    public static List<ulong> mutedUsers = new();
    public static DiscordRole mutedRole;
}
