using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using WinBot.Commands.Attributes;
using WinBot.Misc;

namespace WinBot.Commands.Owner; 

public class EvalCommand : BaseCommandModule {
    [Command("ev")]
    [Description("It's an eval command.")]
    [Usage("[C# Code]")]
    [Attributes.Category(Category.Owner)]
    [RequireOwner]
    public async Task Kill(CommandContext Context, [RemainingText] string code) {
        var author = Context.Message.Author as DiscordMember;
        code = code.Replace("```cs", "");
        code = code.Replace("```", "");
        var OGCode = code;
        try {
            EVGlobals globals = null;
            await Context.Message.Channel.TriggerTypingAsync();
            var scriptOptions = ScriptOptions.Default;

            globals = new EVGlobals {
                Context = Context,
                Bot = Bot.client,
                Commands = Bot.commands,
                Users = UserData.users
            };
            var asms = AppDomain.CurrentDomain
                .GetAssemblies(); // .SingleOrDefault(assembly => assembly.GetName().Name == "MyAssembly");
            foreach (var assembly in asms)
                if ((!assembly.IsDynamic && assembly.FullName.ToLower().Contains("dsharp")) ||
                    assembly.FullName.ToLower().Contains("newtonsoft") ||
                    assembly.FullName.ToLower().Contains("microsoft.csharp") ||
                    assembly.FullName.ToLower().Contains("winbot") || assembly.FullName.ToLower().Contains("scottplot"))
                    scriptOptions = scriptOptions.AddReferences(assembly);
            scriptOptions =
                scriptOptions.AddReferences(
                    "ScottPlot, Version=4.0.48.0, Culture=neutral, PublicKeyToken=86698dc10387c39e");
            scriptOptions.AddReferences(Assembly.GetExecutingAssembly());

            code = @"using System; 
using System.Linq;
using System.IO; 
using System.Threading.Tasks; 
using System.Collections.Generic; 
using System.Text;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using ScottPlot;
using ScottPlot.Drawing;
using WinBot;
using WinBot.Commands.Attributes;" + code;

            var result = await CSharpScript.EvaluateAsync(code, scriptOptions, globals);
            if (result != null) {
                var eb = new DiscordEmbedBuilder();
                eb.WithTitle("Eval");
                eb.WithColor(DiscordColor.Gold);
                eb.WithTimestamp(DateTime.Now);
                eb.AddField("Input", $"```cs\n{OGCode}```");
                eb.AddField("Output", "```cs\n" + result + "```");
                await Context.ReplyAsync("", eb.Build());
            }
        }
        catch (Exception ex) {
            var eb = new DiscordEmbedBuilder();
            eb.WithTitle("Eval");
            eb.WithColor(DiscordColor.Gold);
            eb.WithTimestamp(DateTime.Now);
            eb.AddField("Input", $"```cs\n{OGCode}```");
            eb.AddField("Error", $"```cs\n{ex.Message}```");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}

public class EVGlobals {
    public CommandContext Context { get; set; }
    public DiscordClient Bot { get; set; }
    public CommandsNextExtension Commands { get; set; }
    public List<User> Users { get; set; }
}
