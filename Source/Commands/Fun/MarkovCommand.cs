// NOTE: The code in this file is hilariously shit, I'm just lazy and want this to work lol. Don't care about the quality.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MarkovSharp.TokenisationStrategies;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;
using WinBot.Misc;
using WinBot.Util;
using static WinBot.Util.ResourceManager;

namespace WinBot.Commands.Fun; 

public class MarkovCommand : BaseCommandModule {
    [Command("markov")]
    [Aliases("mk")]
    [Description("Markov chains and things")]
    [Usage("[user] [length]")]
    [Attributes.Category(Category.Fun)]
    public async Task Markov(CommandContext Context, DiscordMember user = null, [RemainingText] int length = 5) {
        List<string> data;
        var sourceName = "Source: User-Submitted Messages";

        if (user == null) {
            data = new List<string>();
            var json = File.ReadAllText(GetResourcePath("randomMessages", ResourceType.JsonData));
            var msgs = JsonConvert.DeserializeObject<List<UserMessage>>(json);
            foreach (var msg in msgs)
                data.Add(msg.content);
        }
        else {
            var bUser = UserData.GetOrCreateUser(user);
            data = bUser.messages;
            sourceName = "Source: " + bUser.username;
        }

        switch (length) {
            case > 25:
                length = 25;
                break;
            
            case < 0:
                length = 1;
                break;
        }
        
        // Generate the markov text
        var model = new StringMarkov(1);
        model.Learn(data);
        model.EnsureUniqueWalk = true;

        var eb = new DiscordEmbedBuilder();
        eb.WithAuthor(sourceName);
        eb.WithColor(DiscordColor.Gold);
        eb.WithFooter(data.Count + " messages in data. Better results will be achieved with more messages.");
        eb.WithDescription(string.Join(' ', model.Walk(length)).Replace("@", "").Truncate(4096));
        await Context.ReplyAsync(eb);
    }
}
