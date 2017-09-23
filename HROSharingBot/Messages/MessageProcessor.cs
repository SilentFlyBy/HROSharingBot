using System;
using System.Threading.Tasks;
using HROSharingBot.Commands;
using HROSharingBot.Sessions;
using Telegram.Bot.Types;

namespace HROSharingBot.Messages
{
    public static class MessageProcessor
    {
        public static async Task ProcessMessage(Message message)
        {
            if (message == null)
                return;

            if (SessionManager.SessionExists(message.Chat.Id))
                ProcessMessageInSession(message, SessionManager.GetSession(message.Chat.Id));
            else
                await ProcessMessageStandAlone(message);
        }

        private static async Task ProcessMessageStandAlone(Message message)
        {
            if (string.IsNullOrEmpty(message.Text))
                return;

            if (message.Chat.Title == "Filesharing")
                return;

            await CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
        }

        private static void ProcessMessageInSession(Message message, Session session)
        {
            session.ExecuteMessage(message);
        }
    }
}