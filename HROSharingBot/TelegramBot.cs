using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Configuration;

namespace HROSharingBot
{
    public static class TelegramBot
    {
        public static readonly TelegramBotClient Bot;


        static TelegramBot()
        {
            Bot = new TelegramBotClient(ConfigReader.Configuration["appSettings:BotToken"]);
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
            await Bot.SendDocumentAsync(chatId, sendFile, text);
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