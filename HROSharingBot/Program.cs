using System;
using System.Threading;
using HROSharingBot.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Telegram.Bot.Args;

namespace HROSharingBot
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        private static void Main()
        {
            //Set cancel key listener
            Console.CancelKeyPress += (sender, args) =>
            {
                QuitEvent.Set();
                args.Cancel = true;
            };
            
            Console.WriteLine("### HRO Sharing Telegram Bot ###");
            TelegramBot.Bot.OnMessage += OnMessageReceived;

            TelegramBot.Bot.StartReceiving();
            Console.WriteLine("Now listening for messages...");

            //Wait for cancel key
            QuitEvent.WaitOne();

            TelegramBot.Bot.StopReceiving();
            Console.WriteLine("Bye.");
        }

        private static async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                MessageProcessor.ProcessMessage(e.Message);
            }
            catch (Exception)
            {
                await TelegramBot.SendMessage(e.Message.Chat.Id, "Ein Fehler ist aufgetreten.");
            }
        }
    }
}