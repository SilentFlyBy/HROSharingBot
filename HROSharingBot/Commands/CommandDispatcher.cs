using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HROSharingBot.Commands
{
    public static class CommandDispatcher
    {
        public enum Command
        {
            Cat,
            Start,
            Exit,
            Undefined
        }

        private static readonly Dictionary<Command, Type> CommandRunners;


        static CommandDispatcher()
        {
            CommandRunners = new Dictionary<Command, Type>
            {
                {Command.Cat, typeof(CatCommandRunner)},
                {Command.Start, typeof(StartCommandRunner)},
                {Command.Exit, typeof(ExitCommandRunner)}
            };
        }

        public static async Task RunCommand(string command, long chatId)
        {
            var commandResult = ParseCommand(command);
            if (commandResult == Command.Undefined)
            {
                await TelegramBot.SendMessage(chatId, "Diesen Befehl kenne ich nicht.");
                return;
            }

            var runner = (ICommandRunner) Activator.CreateInstance(CommandRunners[commandResult]);
            await runner.Run(chatId);
        }

        public static Command ParseCommand(string message)
        {
            if (message == null || !message.StartsWith("/"))
                return Command.Undefined;

            var commandText = message.Substring(1);

            return Enum.TryParse(commandText, true, out Command commandResult) ? commandResult : Command.Undefined;
        }
    }
}