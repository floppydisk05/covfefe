using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Main
{
    public class MinecraftInfoCommand : ModuleBase<SocketCommandContext>
    {
        [Command("mcinfo")]
        [Summary("Gets information on the Minecraft server (WinWorldMC)|")]
        [Priority(Category.Main)]
        public async Task MCInfo()
        {
            await Context.Channel.TriggerTypingAsync();
            
            // Download the server info
            string json = "";
            using(WebClient webClient = new WebClient())
                json = webClient.DownloadString("https://api.mcsrvstat.us/2/mc.winworldpc.com:48666");
            dynamic serverInfo = JsonConvert.DeserializeObject(json);

            // Format the info in an embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Color.Gold);
            
			// Set up the embed
            if((bool)serverInfo.online) {
                eb.WithThumbnailUrl(Context.Guild.IconUrl);
                eb.WithTitle((string)serverInfo.motd.clean[0]);
                eb.AddField("IP", "mc.winworldpc.com:48666", true);
                eb.AddField("Versions", "1.5.2 -> 1.16.5", true);
                eb.AddField("Online?", ((bool)serverInfo.online) ? "Yes" : "No", true);
                eb.AddField("Users Count", $"{(int)serverInfo.players.online}/{(int)serverInfo.players.max}", true);
                eb.AddField("Supports Cracked Accounts?", "No. It never will, just buy the game or stop asking.", true);
            }
            else {
                eb.WithTitle("Server is Offline!");
                eb.WithColor(Color.Red);
            }

            await ReplyAsync("", false, eb.Build());
        }
    }
}