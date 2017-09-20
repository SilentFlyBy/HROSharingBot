using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HROSharingBot.Commands
{
    public interface ICommandRunner
    {
        Task Run(long chatId);
    }
}
