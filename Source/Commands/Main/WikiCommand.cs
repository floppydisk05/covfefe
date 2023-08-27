using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Genbox.Wikipedia;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main; 

public class WikiCommand : BaseCommandModule {
    [Command("wiki")]
    [Description("Search wikipedia")]
    [Usage("[query]")]
    [Attributes.Category(Category.Main)]
    public async Task Wiki(CommandContext Context, [RemainingText] string query) {
        if (string.IsNullOrWhiteSpace(query))
            throw new Exception("You must provide a search query!");

        using var wikiclient = new WikipediaClient();

        var req = new WikiSearchRequest(query);
        req.Limit = 1;

        var resp = await wikiclient.SearchAsync(req);

        foreach (var s in resp.QueryResult.SearchResults) {
            await Context.ReplyAsync($"{s.Url}".Replace(" ", "_"));
            return;
        }

        await Context.ReplyAsync("No results.");
    }
}
