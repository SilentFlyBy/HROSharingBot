using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HROSharingBot.Sessions
{
    public abstract class Session
    {
        public long ChatId { get; set; }


        public Session() { }
        public Session(long chatId)
        {
            this.ChatId = chatId;
        }

        public abstract Task ExecuteMessage(Message message);
    }
}
