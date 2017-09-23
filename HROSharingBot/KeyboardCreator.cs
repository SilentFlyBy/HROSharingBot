using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HROSharingBot
{
    public static class KeyboardCreator
    {
        public static ReplyKeyboardMarkup CreateKeyboard(params string[] values)
        {
            if (values == null) return null;

            var count = (double) values.Length / 2d;
            var rowsCount = Math.Round(count, MidpointRounding.AwayFromZero);

            var buttonArray = new List<KeyboardButton[]>();

            for (var i = 0; i < rowsCount; i++)
            {
                var button2Index = (i * 2) + 1;
                var rowButtons = button2Index < values.Length ? new KeyboardButton[2] : new KeyboardButton[1];
                rowButtons[0] = values[i*2];
                
                if (button2Index < values.Length)
                {
                    rowButtons[1] = values[button2Index];
                }
                buttonArray.Add(rowButtons);
            }

            return new ReplyKeyboardMarkup(buttonArray.ToArray(), false, true);
        }
    }
}