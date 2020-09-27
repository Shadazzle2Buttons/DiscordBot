using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.BotCommands
{
    /// <summary>
    /// Marker interface to distinquish commands that can be run via private message to the bot.
    /// </summary>
    public interface IPrivateMessageCommand : ICommand{ }
}