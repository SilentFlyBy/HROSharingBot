using System.Threading.Tasks;
using HROSharingBot.Sessions;

namespace HROSharingBot.Commands
{
    public class StartCommandRunner : ICommandRunner
    {
        public async Task Run(long chatId)
        {
            var session = SessionManager.CreateSession<UploadFileSession>(chatId);
            if (session != null)
                await TelegramBot.SendMessage(chatId, session.CurrentStep.PromptText, session.CurrentStep.Keyboard);

            else
                await TelegramBot.SendMessage(chatId,
                    "Du bist bereits in einer Sitzung. Gib /exit ein, um abzubrechen.");
        }
    }
}