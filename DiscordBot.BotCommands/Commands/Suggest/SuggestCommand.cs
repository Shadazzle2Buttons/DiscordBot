using System.Linq;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using DiscordBot.StaticValues;

namespace DiscordBot.BotCommands.Commands.Suggest
{
    public class SuggestCommand : IPrivateMessageCommand
    {
        /// <summary>
        /// Sends an anonymized suggestion message. Either publicly in the "Suggestions" channel,
        /// or privately to the specified Caretaker (in case its a suggestion or problem that shouldn't be discussed publicly)
        /// Usage:
        ///     !!suggest public suggestion_text
        /// Or:
        ///     !!suggest private target_username suggestion_text
        /// </summary>
        private readonly DiscordSocketClient _discordClient;

        private const string USAGE_INFO = "```Usage:\n!!suggest public <suggestion text>\nOr:\n!!suggest private <target username> <suggestion text>```";

        public SuggestCommand(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public bool CanExecute(SocketMessage message)
        {
            return message.Content.StartsWith("!!suggest");
        }

        public async Task Execute(SocketMessage message)
        {
            //Limited to users of role "Cow" to avoid abuse
            var userRoles = _discordClient.GetGuild(GuildIds.FortuneFavoured).GetUser(message.Author.Id).Roles;
            if (!userRoles.Any(e => e.Id == RoleIds.Cow))
            {
                await message.Channel.SendMessageAsync("You have no permission to use this command.");
                return;
            }

            string messageContent = message.Content.Replace("!!suggest", "");

            string target = GetSuggestionTarget(messageContent);

            //remove target string from command
            messageContent = messageContent.Trim();
            int index = messageContent.IndexOf(" ");
            messageContent = messageContent.Substring(messageContent.IndexOf(" ") + 1);

            try
            {
                //index == -1 => incomplete command
                if (target == "public" && index != -1)
                {
                    await SendPublicSuggestion(messageContent);
                }
                else if (target == "private" && index != -1)
                {
                    await SendPrivateSuggestion(messageContent);
                }
                else
                {
                    throw new Exception("Invalid suggestion target. Please specify whether the suggestion is to be send **public** or **private**.");
                }

                if (!(message.Channel is IPrivateChannel))  //can't delete DMs
                {
                    await message.DeleteAsync();
                }
            }
            catch (Exception e)
            {
                await message.Channel.SendMessageAsync(e.Message + "\n" + USAGE_INFO);
            }
        }

        private static Embed CreateSuggestionEmbed(string suggestion)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.Color = Color.Green;
            embedBuilder.Title = "Anonymous Suggestion";
            embedBuilder.Description = suggestion;
            return embedBuilder.Build();
        }

        private static string GetSuggestionTarget(string message)
        {
            return message.Trim().Split(" ", 2).First().ToLower();
        }

        private async Task SendPublicSuggestion(string suggestion)
        {
            await (_discordClient.GetChannel(ChannelIds.Suggestions) as ISocketMessageChannel).SendMessageAsync(null, false, CreateSuggestionEmbed(suggestion));
        }

        private async Task SendPrivateSuggestion(string message)
        {
            string[] arrMessage = message.Trim().Split(" ", 2);
            if (arrMessage.Length < 2)
            {
                throw new Exception("Missing target username.");
            }
            string targetUsername = arrMessage[0];
            string suggestion = arrMessage[1];


            var role = _discordClient.GetGuild(GuildIds.FortuneFavoured).GetRole(RoleIds.Caretaker);

            var targetUser = FindTargetUserInRole(role, targetUsername);
            if (targetUser == null)
            {
                throw new Exception("User " + targetUsername + " is not in group " + role.Name + ".");
            }

            var dmChannel = await targetUser.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(null, false, CreateSuggestionEmbed(suggestion));
            await dmChannel.CloseAsync();

            return;
        }

        private SocketUser FindTargetUserInRole(SocketRole role, string targetUsername)
        {
            foreach (var member in role.Members)
            {
                if (!string.IsNullOrEmpty(member.Nickname))
                {
                    if (member.Nickname.ToLower() == targetUsername.ToLower())
                    {
                        return member;
                    }
                }

                if (member.Username.ToLower() == targetUsername.ToLower())
                {
                    return member;
                }
            }
            return null;
        }
    }
}
