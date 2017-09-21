using System;
using System.Threading;
using HROSharingBot.Messages;
using Telegram.Bot.Args;

namespace HROSharingBot
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        private static void Main()
        {
            Console.CancelKeyPress += (sender, args) =>
            {
                QuitEvent.Set();
                args.Cancel = true;
            };
            
            Console.WriteLine("Starting HRO Sharing Telegram Bot.");
            TelegramBot.Bot.OnMessage += OnMessageReceived;

            TelegramBot.Bot.StartReceiving();
            Console.WriteLine("Now listening for messages...");

            QuitEvent.WaitOne();

            Console.WriteLine("Exiting.");
            TelegramBot.Bot.StopReceiving();
        }

        private static async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                MessageProcessor.ProcessMessage(e.Message);
            }
            catch (Exception)
            {
                await TelegramBot.WriteMessage(e.Message.Chat.Id, "Ein Fehler ist aufgetreten.");
            }
        }
    }
}