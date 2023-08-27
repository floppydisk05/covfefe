using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using WinBot.Commands.Attributes;
using WinBot.Util;
using static WinBot.Util.ResourceManager;

namespace WinBot.Commands.Fun; 

public class MessageCommand : BaseCommandModule {
    public List<UserMessage> messages;

    [Command("msg")]
    [Description("Get a random user-submitted message")]
    [Usage("add (or leave blank)")]
    [Attributes.Category(Category.Fun)]
    public async Task Message(CommandContext Context, [RemainingText] string textArgs = null) {
        var jsonFile = GetResourcePath("randomMessages", ResourceType.JsonData);

        // Load messages from json if not already done
        if (messages == null) {
            if (File.Exists(jsonFile)) {
                messages = JsonConvert.DeserializeObject<List<UserMessage>>(File.ReadAllText(jsonFile));
            }
            else {
                messages = new List<UserMessage>();
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(messages, Formatting.Indented));
            }
        }

        // Bypass first few if statements
        if (string.IsNullOrWhiteSpace(textArgs))
            textArgs = "WHAT H";

        // TODO: Clean this up by unifying the embed sending code
        // No arguments; send a random image
        // Specific-ID
        if (messages.FirstOrDefault(x => x.ID == textArgs) != null) {
            // Get the message
            var msg = messages.FirstOrDefault(x => x.ID == textArgs);

            // Create and send the embed
            var eb = new DiscordEmbedBuilder();
            eb.WithAuthor(msg.author, null, msg.avatarUrl);
            eb.WithDescription(msg.content);
            eb.WithTimestamp(msg.sentAt);
            eb.WithFooter(
                $"ID: {msg.ID}\nSubmitted by: {msg.submitter}\nSubmit your own with the \"{Bot.config.prefix}msg add\" command");
            eb.WithColor(DiscordColor.Gold);
            await Context.Channel.SendMessageAsync(eb);
        }
        // Add
        else if (textArgs.ToLower() == "add") {
            var referencedMessage = Context.Message.ReferencedMessage;
            if (referencedMessage == null) {
                var messages = await Context.Channel.GetMessagesAsync(5);
                foreach (var msg in messages) {
                    // Don't grab the author's own message,
                    // they shouldn't be self-absorbed like that.
                    if (msg.Author.Id == Context.User.Id)
                        continue;
                    if (string.IsNullOrWhiteSpace(msg.Content))
                        continue;
                    referencedMessage = msg;
                    break;
                }
            }

            if (referencedMessage == null || referencedMessage.Author.Id == Context.User.Id)
                throw new Exception(
                    "Invalid or no message, message must've been sent in the past 5 messages and not be from the command executor.");
            if (string.IsNullOrWhiteSpace(referencedMessage.Content))
                throw new Exception("Message must have content!");

            // Add the message; this code sucks lol
            var newMessage = new UserMessage();
            newMessage.author = referencedMessage.Author.Username;
            newMessage.authorID = referencedMessage.Author.Id;
            newMessage.avatarUrl = referencedMessage.Author.GetAvatarUrl(ImageFormat.Png);
            newMessage.channel = referencedMessage.Channel.Name;
            newMessage.channelID = referencedMessage.ChannelId;
            newMessage.content = referencedMessage.Content;
            newMessage.messageID = referencedMessage.Id;
            newMessage.sentAt = referencedMessage.CreationTimestamp.DateTime.ToLocalTime();
            newMessage.submitter = Context.User.Username;
            idRecalc:
            var idInt = new Random().Next(0, 0xFFFF);
            var ID = Convert.ToBase64String(BitConverter.GetBytes(idInt));
            if (messages.FirstOrDefault(x => x.ID == ID) != null)
                goto idRecalc;
            newMessage.ID = ID;
            messages.Add(newMessage);

            File.WriteAllText(jsonFile, JsonConvert.SerializeObject(messages, Formatting.Indented));
            await Context.ReplyAsync($"Successfully added your message! ID: `{ID}`");
        }
        else if (textArgs.ToLower().Split(" ").FirstOrDefault() == "del") {
            if (!Bot.client.CurrentApplication.Owners.Contains(Context.User))
                throw new Exception("You lack the sufficient permissions to run this command");
            var message = textArgs.Split(" ").LastOrDefault();
            if (message == null)
                throw new Exception("You must provide a message ID!");

            var messageToRemove = messages.FirstOrDefault(x => x.ID == message);
            if (messageToRemove == null)
                throw new Exception("You must provide a valid message ID!");

            messages.Remove(messageToRemove);
            File.WriteAllText(jsonFile, JsonConvert.SerializeObject(messages, Formatting.Indented));
            await Context.ReplyAsync($"Successfully removed `{message}`!");
        }
        else if (textArgs.ToLower() == "count") {
            await Context.ReplyAsync($"There are {messages.Count} messages.");
        }
        else {
            if (messages.Count <= 0)
                throw new Exception(
                    $"There are no messages! Use the \"{Bot.config.prefix}msg add\" command to add one!");

            // Get random image
            var msg = messages[new Random().Next(0, messages.Count)];

            // Create and send the embed
            var eb = new DiscordEmbedBuilder();
            eb.WithAuthor(msg.author, null, msg.avatarUrl);
            eb.WithDescription(msg.content);
            eb.WithTimestamp(msg.sentAt);
            eb.WithFooter(
                $"ID: {msg.ID}\nSubmitted by: {msg.submitter}\nSubmit your own with the \"{Bot.config.prefix}msg add\" command");
            eb.WithColor(DiscordColor.Gold);
            await Context.Channel.SendMessageAsync(eb);
        }
    }
}

public class UserMessage {
    public string author { get; set; }
    public string avatarUrl { get; set; }
    public string channel { get; set; }
    public string content { get; set; }
    public string submitter { get; set; }
    public string ID { get; set; }
    public ulong authorID { get; set; }
    public ulong messageID { get; set; }
    public ulong channelID { get; set; }
    public DateTime sentAt { get; set; }
}
