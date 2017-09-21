using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HROSharingBot.Sessions
{
    public abstract class Session
    {
        protected Session()
        {
        }

        public long ChatId { get; set; }

        public abstract Task ExecuteMessage(Message message);
    }
}