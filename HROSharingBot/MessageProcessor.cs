using HROSharingBot.Commands;
using HROSharingBot.Sessions;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace HROSharingBot
{
    public static class MessageProcessor
    {
        public static void ProcessMessage(Message message)
        {
            if (message == null)
                return;

            if (SessionManager.SessionExists(message.Chat.Id))
            {
                ProcessMessageInSession(message, SessionManager.GetSession(message.Chat.Id));
            }
            else
            {
                ProcessMessageStandAlone(message);
            }
        }

        private static void ProcessMessageStandAlone(Message message)
        {
            if (String.IsNullOrEmpty(message.Text))
                return;

            if (message.Chat.Title == "Filesharing")
                return;


            CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
        }

        private static void ProcessMessageInSession(Message message, Session session)
        {
            if (CommandDispatcher.ParseCommand(message.Text) == CommandDispatcher.Command.Exit)
            {
                SessionManager.DestroySession(session);
                return;
            }

            var executeTask = session.ExecuteMessage(message);
        }
    }
}
