using System.Threading.Tasks;
using HROSharingBot.Commands;
using HROSharingBot.Sessions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HROSharingBot.Messages
{
    public static class MessageProcessor
    {
        public static async Task ProcessMessage(Message message)
        {
            if (message == null)
                return;

            if (SessionManager.SessionExists(message.Chat.Id))
                await ProcessMessageInSession(message, SessionManager.GetSession(message.Chat.Id));
            else
                await ProcessMessageStandAlone(message);
        }

        private static async Task ProcessMessageStandAlone(Message message)
        {
            if (string.IsNullOrEmpty(message.Text))
                return;

            if (message.Chat.Type == ChatType.Group || message.Chat.Type == ChatType.Supergroup)
                return;

            await CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
        }

        private static async Task ProcessMessageInSession(Message message, Session session)
        {
            if (CommandDispatcher.ParseCommand(message.Text) != CommandDispatcher.Command.Undefined)
            {
                await CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
                return;
            }
            
            await session.ExecuteMessage(message);
        }
    }
}