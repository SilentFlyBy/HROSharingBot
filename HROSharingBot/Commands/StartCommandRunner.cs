using HROSharingBot.Sessions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HROSharingBot.Commands
{
    public class StartCommandRunner : ICommandRunner
    {
        public async Task Run(long chatId)
        {
            var session = SessionManager.CreateSession<UploadFileSession>(chatId);
            await TelegramBot.WriteMessage(chatId, session.CurrentStep.PromptText);
        }
    }
}
