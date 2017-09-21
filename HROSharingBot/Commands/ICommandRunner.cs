using System.Threading.Tasks;

namespace HROSharingBot.Commands
{
    public interface ICommandRunner
    {
        Task Run(long chatId);
    }
}