using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HROSharingBot.Sessions
{
    public class UploadFileSession : Session
    {
        public UploadStepDesciptor CurrentStep
        {
            get
            {
                var step = (UploadFileSessionStep)sessionStep;

                return steps.Where(s => s.Step == step).FirstOrDefault();
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Platforms { get; set; } = new List<string>();
        public string ImageLink { get; set; }
        private string _fileid;
        public string FileId { get; set; }


        private int sessionStep;
        private List<UploadStepDesciptor> steps;

        public UploadFileSession() : base()
        {
            steps = new List<UploadStepDesciptor>();
            steps.Add(new UploadStepDesciptor()
            {
                Step = UploadFileSessionStep.SetTitle,
                PromptText = "Bitte gib den Namen des Paketes ein.",
                Conditions = new MessageVerifier.MessageCondition()
                {
                    Text = new MessageVerifier.TextCondition() { MinLength = 1, MaxLength = 200 }
                },
                InvalidMessageErrorText = "Der Titel muss zwischen 1 und 200 Zeichen lang sein.",
                PropertyName = nameof(this.Title)
            });
            steps.Add(new UploadStepDesciptor()
            {
                Step = UploadFileSessionStep.SetDescription,
                PromptText = "Bitte gib die Beschreibung des Paketes ein.",
                Conditions = new MessageVerifier.MessageCondition()
                {
                    Text = new MessageVerifier.TextCondition() { MinLength = 1, MaxLength = 5000 }
                },
                InvalidMessageErrorText = "Die Beschreibung muss zwischen 1 und 5000 Zeichen lang sein.",
                PropertyName = nameof(this.Description)
            });
            steps.Add(new UploadStepDesciptor()
            {
                Step = UploadFileSessionStep.SetPlatform,
                PromptText = "Bitte lege die Plattform für das Paket fest.",
                Conditions = new MessageVerifier.MessageCondition()
                {
                    Text = new MessageVerifier.TextCondition() { EqualTo = new string[] { "Windows", "Linux", "OSX", "Android", "IOS", "Andere" } }
                },
                InvalidMessageErrorText = "Bitte gib eine der folgenden Plattformen an: Windows, Linux, OSD, Android, IOS, Andere",
                PropertyName = nameof(this.Platforms)
            });
            steps.Add(new UploadStepDesciptor()
            {
                Step = UploadFileSessionStep.SetImage,
                PromptText = "Bitte gib den Link zu einem Bild für das Paket an.",
                Conditions = new MessageVerifier.MessageCondition()
                {
                    Text = new MessageVerifier.TextCondition() { MinLength = 1, Contains = new string[] { "http", "none" } }
                },
                InvalidMessageErrorText = "Dies ist kein gültiger Link",
                PropertyName = nameof(this.ImageLink)
            });
            steps.Add(new UploadStepDesciptor()
            {
                Step = UploadFileSessionStep.UploadFile,
                PromptText = "Bitte lade nun das Paket hoch.",
                Conditions = new MessageVerifier.MessageCondition()
                {
                    File = new MessageVerifier.FileCondition() { Null = false }
                },
                InvalidMessageErrorText = "Dies ist keine gültige Datei",
                PropertyName = nameof(this.FileId)
            });
        }


        public override async Task ExecuteMessage(Message message)
        {
            if (!message.Verify(CurrentStep.Conditions))
            {
                await TelegramBot.WriteMessage(this.ChatId, "Ungültige Eingabe. " + CurrentStep.InvalidMessageErrorText);
                return;
            }


            var propinfo = typeof(UploadFileSession).GetProperty(CurrentStep.PropertyName);


            if (propinfo.PropertyType == typeof(string))
            {
                propinfo.SetValue(this, message.Text);
            }
            else if (propinfo.PropertyType == typeof(List<string>))
            {
                object list = propinfo.GetValue(this, null);
                propinfo.PropertyType.GetMethod("Add").Invoke(list, new[] { message.Text });
            }

            if (propinfo.Name == nameof(this.FileId))
            {
                if (message.Document.FileId != null)
                    propinfo.SetValue(this, message.Document.FileId);
            }

            if (sessionStep == Enum.GetNames(typeof(UploadFileSessionStep)).Length-1)
            {
                await MakeFileUploadMessageAsync();
                SessionManager.DestroySession(this);
                return;
            }

            NextStep();

            await TelegramBot.WriteMessage(this.ChatId, CurrentStep.PromptText);
        }

        private async Task MakeFileUploadMessageAsync()
        {
            while(FileId == null) { }
            string text = "";
            text += "Titel: " + this.Title;


            text += "\n\n";

            text += "Beschreibung: " + this.Description;

            text += "\n\n";

            text += "Plattform: ";
            foreach(var p in this.Platforms)
            {
                text += p + " ";
            }

            text += "\n\n";

            text += "Bild: " + this.ImageLink;


            await TelegramBot.WriteMessage(-217390667, text);
            var file = new FileToSend(this.FileId);
            await TelegramBot.Bot.SendDocumentAsync(-217390667, file);
        }

        private void NextStep()
        {
            this.sessionStep++;
        }



        public enum UploadFileSessionStep
        {
            SetTitle = 0,
            SetDescription = 1,
            SetPlatform = 2,
            SetImage = 3,
            UploadFile = 4
        }

        public class UploadStepDesciptor
        {
            public UploadFileSessionStep Step { get; set; }
            public string PromptText { get; set; }
            public MessageVerifier.MessageCondition Conditions { get; set; }
            public string InvalidMessageErrorText { get; set; }
            public string PropertyName { get; set; }
        }
    }
}
