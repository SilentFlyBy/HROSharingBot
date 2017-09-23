using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HROSharingBot.Commands
{
    public class CatCommandRunner : ICommandRunner
    {
        public async Task Run(long chatId)
        {
            const string apiUrl = "http://thecatapi.com/api/images/get";

            using (var http = new HttpClient())
            using (var contentStream = await http.GetStreamAsync(apiUrl))
            using (var ms = new MemoryStream())
            {
                contentStream.CopyTo(ms);
                ms.Position = 0;

                await TelegramBot.SendImageMessage(chatId, "", ms);
            }
        }
    }
}