using System;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.NerdStuff; 

public class WeatherDataCommand : BaseCommandModule {
    [Command("weatherdata")]
    [Description("Pulls data from floppy's weather station")]
    [Usage("(longterm)")]
    [Attributes.Category(Category.Main)]
    public async Task WeatherData(CommandContext Context, string command = null) {
        var eb = new DiscordEmbedBuilder();
        if (command == null) {
            var json = "";
            var time = "";

            // Try to grab the json string from the API
            try {
                using (var client = new WebClient()) {
                    json = client.DownloadString("http://diskfloppy.me/api/weather");
                }
            }
            catch {
                eb.WithTitle("Unable to retrieve weather data!");
                eb.WithColor(new DiscordColor("#ED4245"));
                return;
            }

            if (json != "") {
                dynamic data = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object
                DateTimeOffset updated = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(data.updated));

                // Send the bracelettes in an embed
                eb.WithTitle("floppy's Weather Data");
                eb.WithColor(DiscordColor.Gold);
                eb.WithTimestamp(updated);
                // F i e l d s
                eb.AddField("Outside Temp",
                    $"{data.current.temperature}°C ({data.current.temperature * (9 / 5) + 32}°F)", true);
                eb.AddField("Outside Humidity", $"{data.current.humidity}%", true);
                eb.AddField("Precipitation", $"{data.current.rain_rate} mm/hr ({data.current.rain_rate / 25.4} in/hr)",
                    true);
                if (Convert.ToInt32(data.current.wind.speed) == 0) eb.AddField("Wind Direction", "No wind", true);
                else
                    eb.AddField("Wind Direction/Speed",
                        @$"{data.current.wind.speed} mph ({data.current.wind.direction.degrees}°, {data.current.wind.direction.cardinal}, {Math.Round((double)data.current.wind.speed / 1.151, 2)} knots)",
                        true);
            }
        }
        else if (command == "longterm") {
            var json = "";
            var time = "";

            // Try to grab the json string from the API
            try {
                using (var client = new WebClient()) {
                    json = client.DownloadString("http://diskfloppy.me/api/weather");
                }
            }
            catch {
                eb.WithTitle("Unable to retrieve weather data!");
                eb.WithColor(new DiscordColor("#ED4245"));
                return;
            }

            if (json != "") {
                dynamic data = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object
                DateTimeOffset updated = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(data.updated));

                eb.WithTitle("Long-Term Data");
                eb.WithColor(DiscordColor.Gold);
                eb.AddField("Past Day of Rainfall", $"{data.longterm.rain.day} mm");
                eb.AddField("Past Month of Rainfall", $"{data.longterm.rain.month} mm");
                eb.AddField("Past Year of Rainfall", $"{data.longterm.rain.year} mm");
                eb.WithTimestamp(updated);
            }
        }
        else {
            return;
        }

        await Context.ReplyAsync("", eb.Build());
    }
}
