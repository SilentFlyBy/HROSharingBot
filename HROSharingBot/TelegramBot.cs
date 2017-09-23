using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;

namespace HROSharingBot
{
    public static class TelegramBot
    {
        private static readonly TelegramBotClient Bot;
        public static event EventHandler<MessageEventArgs> OnMessage;


        static TelegramBot()
        {
            Bot = new TelegramBotClient(ConfigReader.Configuration["appSettings:BotToken"]);
            Bot.OnMessage += (e, args) =>
            {
                OnMessage?.Invoke(e, args);
            };
        }

        public static void StartReceiving()
        {
            Bot.StartReceiving();
        }

        public static void StopReceiving()
        {
            Bot.StopReceiving();
        }

        
        public static async Task SendMessage(long chatId, string text)
        {
            await Bot.SendTextMessageAsync(chatId, text);
        }
        public static async Task SendMessage(long chatId, string text, ReplyKeyboardMarkup markup)
        {
            if (markup != null)
                await SendButtonMessage(chatId, text, markup);
            
            else
                await Bot.SendTextMessageAsync(chatId, text);
        }

        public static async Task SendImageMessage(long chatId, string text, Stream image)
        {
            var photo = new FileToSend("Bild", image);
            await Bot.SendPhotoAsync(chatId, photo, text);
        }

        public static async Task SendFileMessage(long chatId, string text, Stream file)
        {
            var sendFile = new FileToSend("Datei", file);
            await SendFileMessage(chatId, text, sendFile);
        }

        public static async Task SendFileMessage(long chatId, string text, FileToSend file)
        {
            await Bot.SendDocumentAsync(chatId, file, text);
        }

        public static async Task ForwardMessage(long fromChatId, long toChatId, int messageId)
        {
            await Bot.ForwardMessageAsync(toChatId, fromChatId, messageId);
        }

        private static async Task SendButtonMessage(long chatId, string text, IReplyMarkup markup)
        {
            await Bot.SendTextMessageAsync(chatId, text, replyMarkup: markup);
        }
    }
}