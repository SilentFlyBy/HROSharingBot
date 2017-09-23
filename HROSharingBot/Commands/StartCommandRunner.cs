﻿using System.Threading.Tasks;
using HROSharingBot.Sessions;

namespace HROSharingBot.Commands
{
    public class StartCommandRunner : ICommandRunner
    {
        public async Task Run(long chatId)
        {
            var session = SessionManager.CreateSession<UploadFileSession>(chatId);
            if(session != null)
                await TelegramBot.SendMessage(chatId, session.CurrentStep.PromptText, session.CurrentStep.Keyboard);
        }
    }
}