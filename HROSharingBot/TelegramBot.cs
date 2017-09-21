using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HROSharingBot
{
    public static class TelegramBot
    {
        public static readonly TelegramBotClient Bot =
            new TelegramBotClient("424327957:AAF68jiSHTF24a5Uv5EGtxZX57bretFTyTI");

        public static async Task WriteMessage(long chatId, string text)
        {
            await Bot.SendTextMessageAsync(chatId, text);
        }

        public static async Task WriteImageMessage(long chatId, string text, Stream image)
        {
            var photo = new FileToSend("Bild", image);
            await Bot.SendPhotoAsync(chatId, photo, text);
        }

        public static async Task WriteFileMessage(long chatId, string text, Stream file)
        {
            var sendFile = new FileToSend("Datei", file);
            await Bot.SendDocumentAsync(chatId, sendFile, text);
        }

        public static async Task ForwardMessage(long fromChatId, long toChatId, int messageId)
        {
            await Bot.ForwardMessageAsync(toChatId, fromChatId, messageId);
        }
    }
}