using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;

namespace HROSharingBot.Messages
{
    public static class MessageVerifier
    {
        public static bool Verify(this Message message, MessageCondition condition)
        {
            var textVerified = VerifyText(message.Text, condition.Text);
            var imageVerified = VerifyImage(message.Photo, condition.Image);
            var fileVerified = VerifyFile(message.Document, condition.File);

            return textVerified && imageVerified && fileVerified;
        }

        private static bool VerifyFile(File document, FileCondition condition)
        {
            if (condition?.Null == null) return true;
            if (condition.Null.Value)
                if (!(string.IsNullOrEmpty(document?.FilePath) || document.FileSize == 0 || document.FileStream != null))
                    return false;

            if (condition.Null.Value) return true;
            return document != null && document.FileSize > 0;
        }

        private static bool VerifyImage(IReadOnlyCollection<PhotoSize> photo, ImageCondition condition)
        {
            if (condition?.Null == null) return true;
            if (condition.Null.Value)
                if (photo.Count != 0)
                    return false;

            if (condition.Null.Value) return true;
            return photo != null && photo.Count > 0;
        }

        private static bool VerifyText(string text, TextCondition condition)
        {
            if (condition == null)
                return true;

            if (condition.EqualTo != null && condition.EqualTo.Length > 0)
            {
                var verify = false;
                foreach (var equal in condition.EqualTo)
                    if (text == equal)
                        verify = true;

                if (!verify)
                    return false;
            }

            if (condition.Contains != null && condition.Contains.Length > 0)
            {
                var verify = false;
                foreach (var contain in condition.Contains)
                    if (text.Contains(contain))
                        verify = true;
                if (!verify)
                    return false;
            }

            if (condition.MinLength > 0)
                if (!(text.Length >= condition.MinLength))
                    return false;

            if (condition.MaxLength <= 0) return true;
            return text.Length <= condition.MaxLength;
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class MessageCondition
        {
            public TextCondition Text { get; set; }
            public ImageCondition Image { get; set; }
            public FileCondition File { get; set; }
        }

        public class TextCondition
        {
            public int MaxLength { get; set; }
            public int MinLength { get; set; }
            public string[] EqualTo { get; set; }
            public string[] Contains { get; set; }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public abstract class ImageCondition
        {
            public bool? Null { get; set; }
        }

        public class FileCondition
        {
            public bool? Null { get; set; }
        }
    }
}