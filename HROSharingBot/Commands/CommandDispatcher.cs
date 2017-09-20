using System;
using System.Collections.Generic;
using System.Text;

namespace HROSharingBot.Commands
{
    public static class CommandDispatcher
    {
        private static Dictionary<Command, Type> commandRunners;


        static CommandDispatcher()
        {
            commandRunners = new Dictionary<Command, Type>();
            commandRunners.Add(Command.Cat, typeof(CatCommandRunner));
            commandRunners.Add(Command.Start, typeof(StartCommandRunner));
        }

        public static void RunCommand(string command, long chatId)
        {
            var commandResult = ParseCommand(command);
            if(commandResult == Command.Undefined)
            {
                var sendTask = TelegramBot.WriteMessage(chatId, "Diesen Befehl kenne ich nicht.");
                return;
            }

            ICommandRunner runner = (ICommandRunner)Activator.CreateInstance(commandRunners[commandResult]);
            var runTask = runner.Run(chatId);
        }

        public static Command ParseCommand(string message)
        {
            if (message == null)
                return Command.Undefined;

            if (message.StartsWith("/"))
            {
                var commandText = message.Substring(1);
                Command commandResult;

                if(Enum.TryParse(commandText, true, out commandResult))
                {
                    return commandResult;
                }
            }

            return Command.Undefined;
        }



        public enum Command
        {
            Cat,
            Start,
            Exit,
            Undefined
        }
    }
}
