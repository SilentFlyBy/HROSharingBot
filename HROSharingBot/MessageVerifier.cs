using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace HROSharingBot
{
    public static class MessageVerifier
    {
        public static bool Verify(this Message message, MessageCondition condition)
        {
            bool textVerified = VerifyText(message.Text, condition.Text);
            bool imageVerified = VerifyImage(message.Photo, condition.Image);
            bool fileVerified = VerifyFile(message.Document, condition.File);

            return textVerified && imageVerified && fileVerified;
        }

        private static bool VerifyFile(Document document, FileCondition condition)
        {
            if (condition == null)
                return true;

            if (condition.Null != null)
            {
                if(condition.Null.Value == true)
                {
                    if (!(document == null || String.IsNullOrEmpty(document.FilePath) || document.FileSize == 0 || document.FileStream != null))
                        return false;
                }

                if(condition.Null.Value == false)
                {
                    if (!(document != null && document.FileSize > 0))
                        return false;
                }
            }

            return true;
        }

        private static bool VerifyImage(PhotoSize[] photo, ImageCondition condition)
        {
            if (condition == null)
                return true;

            if(condition.Null != null)
            {
                if(condition.Null.Value == true)
                {
                    if (!(photo.Length == 0 || photo == null))
                        return false;
                }

                if(condition.Null.Value == false)
                {
                    if (!(photo != null && photo.Length > 0))
                        return false;
                }
            }

            return true;
        }

        private static bool VerifyText(string text, TextCondition condition)
        {
            if (condition == null)
                return true;

            if (condition.EqualTo != null && condition.EqualTo.Length > 0)
            {
                bool verify = false;
                foreach(var equal in condition.EqualTo)
                {
                    if (text == equal)
                        verify = true;
                }

                if (!verify)
                    return false;
            }

            if (condition.Contains != null && condition.Contains.Length > 0)
            {
                bool verify = false;
                foreach(var contain in condition.Contains)
                {
                    if (text.Contains(contain))
                        verify = true;
                }
                if (!verify)
                    return false;
            }

            if (condition.MinLength > 0)
            {
                if (!(text.Length >= condition.MinLength))
                    return false;
            }

            if (condition.MaxLength > 0)
            {
                if (!(text.Length <= condition.MaxLength))
                    return false;
            }

            return true;
        }

        public class MessageCondition
        {
            public TextCondition Text { get; set; }
            public ImageCondition Image { get; set; }
            public FileCondition File { get; set; }
        }

        public class TextCondition
        {
            public int MaxLength { get; set; } = 0;
            public int MinLength { get; set; } = 0;
            public string[] EqualTo { get; set; } = null;
            public string[] Contains { get; set; } = null;
        }

        public class ImageCondition
        {
            public bool? Null { get; set; }
        }

        public class FileCondition
        {
            public bool? Null { get; set; }
        }
    }
}
