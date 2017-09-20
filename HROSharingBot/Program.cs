using HROSharingBot.Commands;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace HROSharingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBot.Bot.OnMessage += OnMessageReceived;

            TelegramBot.Bot.StartReceiving();

            Console.ReadLine();

            TelegramBot.Bot.StopReceiving();
        }

        private static void OnMessageReceived(object sender, MessageEventArgs e)
        {
            MessageProcessor.ProcessMessage(e.Message);
        }
    }
}