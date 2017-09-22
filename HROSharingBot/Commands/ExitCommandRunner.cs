using System.Threading.Tasks;
using HROSharingBot.Sessions;

namespace HROSharingBot.Commands
{
    public class ExitCommandRunner : ICommandRunner
    {
        public async Task Run(long chatId)
        {
            if (SessionManager.SessionExists(chatId))
            {
                SessionManager.DestroySession(SessionManager.GetSession(chatId));
                await TelegramBot.SendMessage(chatId, "Abgebrochen");
            }
        }
    }
}