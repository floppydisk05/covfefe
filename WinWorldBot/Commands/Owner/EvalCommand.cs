using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WinWorldBot.Commands
{
    public class EvalCommand : ModuleBase<SocketCommandContext>
    {
        [Command("eval"), Alias("ev")]
        [Summary("It's an eval command <:norton:767557055694635018>|[Code]")]
        [Priority(Category.Owner)]
        private async Task Eval([Remainder] string code)
        {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) {
                await Context.Message.DeleteAsync();
                return;
            }
            code = code.Replace("```cs", "");
            code = code.Replace("```", "");
            string OGCode = code;
            try
            {
                EVGlobals globals = null;
                await Context.Message.Channel.TriggerTypingAsync();
                var scriptOptions = ScriptOptions.Default;


                globals = new EVGlobals()
                {
                    Context = Context,
                    Bot = Bot.client,
                    Commands = Bot.commands,
                    Services = Bot.services,
                    BConfig = Bot.config
                };
                var asms = AppDomain.CurrentDomain.GetAssemblies(); // .SingleOrDefault(assembly => assembly.GetName().Name == "MyAssembly");
                foreach (Assembly assembly in asms)
                {
                    if (!assembly.IsDynamic && assembly.FullName.ToLower().Contains("discord") || assembly.FullName.ToLower().Contains("newtonsoft") || assembly.FullName.ToLower().Contains("microsoft.csharp") || assembly.FullName.ToLower().Contains("lastfm"))
                    {
                        scriptOptions = scriptOptions.AddReferences(assembly);
                    }
                }

                code = @"using System; 
using System.Linq;
using System.IO; 
using System.Threading.Tasks; 
using System.Collections.Generic; 
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;" + code;

                var result = await CSharpScript.EvaluateAsync(code, scriptOptions, globals);
                if (result != null)
                {
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.WithTitle("Eval");
                    eb.WithColor(Bot.config.embedColour);
                    eb.WithCurrentTimestamp();
                    eb.AddField("Input", $"```cs\n{OGCode}```");
                    eb.AddField("Output", $"```cs\n" + result + "```");
                    await ReplyAsync("", false, eb.Build());
                }

            }
            catch (Exception ex)
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle("Eval");
                eb.WithColor(Bot.config.embedColour);
                eb.WithCurrentTimestamp();
                eb.AddField("Input", $"```cs\n{OGCode}```");
                eb.AddField("Error", $"```cs\n{ex.Message}```");
                await ReplyAsync($"", false, eb.Build());
            }
        }

    }

    /// <summary>
    /// Everything that is passed into evals
    /// </summary>
    public class EVGlobals
    {
        public SocketCommandContext Context { get; set; }
        public DiscordSocketClient Bot { get; set; }
        public IServiceProvider Services { get; set; }
        public CommandService Commands { get; set; }
        public BotConfig BConfig { get; set; }
    }
}