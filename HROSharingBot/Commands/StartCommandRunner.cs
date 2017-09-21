using System.Threading.Tasks;
using HROSharingBot.Sessions;

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