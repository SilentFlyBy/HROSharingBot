using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HROSharingBot.Commands;
using HROSharingBot.Messages;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HROSharingBot.Sessions
{
    public class UploadFileSession : Session
    {
        private const long GroupChatId = -217390667;


        private int _sessionStep;
        private readonly List<UploadStepDesciptor> _steps;

        public UploadFileSession()
        {
            _steps = new List<UploadStepDesciptor>
            {
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.SetTitle,
                    PromptText = "Bitte gib den Namen des Paketes ein.",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        Text = new MessageVerifier.TextCondition {MinLength = 1, MaxLength = 200}
                    },
                    InvalidMessageErrorText = "Der Titel muss zwischen 1 und 200 Zeichen lang sein.",
                    Action = SetTitle
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.SetDescription,
                    PromptText = "Bitte gib die Beschreibung des Paketes ein.",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        Text = new MessageVerifier.TextCondition {MinLength = 1, MaxLength = 5000}
                    },
                    InvalidMessageErrorText = "Die Beschreibung muss zwischen 1 und 5000 Zeichen lang sein.",
                    Action = SetDescription
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.SetPlatform,
                    PromptText = "Bitte lege die Plattform für das Paket fest.",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        Text = new MessageVerifier.TextCondition
                        {
                            EqualTo = new[] {"Windows", "Linux", "OSX", "Android", "IOS", "Andere"}
                        }
                    },
                    InvalidMessageErrorText =
                        "Bitte gib eine der folgenden Plattformen an: Windows, Linux, OSD, Android, IOS, Andere",
                    Action = SetPlatform
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.MorePlatforms,
                    PromptText = "Möchtest du weitere Plattformen festlegen?",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        Text = new MessageVerifier.TextCondition {EqualTo = new[] {"Ja", "Nein"}}
                    },
                    InvalidMessageErrorText = "Bitte gib 'Ja' oder 'Nein' ein.",
                    Action = MorePlatforms
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.SetImage,
                    PromptText = "Bitte gib den Link zu einem Bild für das Paket an.",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        Text = new MessageVerifier.TextCondition {MinLength = 1, Contains = new[] {"http", "none"}}
                    },
                    InvalidMessageErrorText = "Dies ist kein gültiger Link",
                    Action = SetImage
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.UploadFile,
                    PromptText = "Bitte lade nun das Paket hoch.",
                    Conditions = new MessageVerifier.MessageCondition
                    {
                        File = new MessageVerifier.FileCondition {Null = false}
                    },
                    InvalidMessageErrorText = "Dies ist keine gültige Datei",
                    Action = UploadFile
                },
                new UploadStepDesciptor
                {
                    Step = UploadFileSessionStep.Finalize,
                    PromptText = "",
                    Conditions = new MessageVerifier.MessageCondition(),
                    InvalidMessageErrorText = "",
                    Action = Finalize
                }
            };
        }

        public UploadStepDesciptor CurrentStep
        {
            get
            {
                var step = (UploadFileSessionStep) _sessionStep;

                return _steps.FirstOrDefault(s => s.Step == step);
            }
        }

        private string Title { get; set; }
        private string Description { get; set; }
        private List<string> Platforms { get; set; } = new List<string>();
        private string ImageLink { get; set; }
        private string FileId { get; set; }

        public override async Task ExecuteMessage(Message message)
        {
            if (CommandDispatcher.ParseCommand(message.Text) != CommandDispatcher.Command.Undefined)
            {
                CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
                return;
            }

            if (!message.Verify(CurrentStep.Conditions))
            {
                await TelegramBot.WriteMessage(ChatId, "Ungültige Eingabe. " + CurrentStep.InvalidMessageErrorText);
                return;
            }

            CurrentStep.Action(message);

            if (_sessionStep == Enum.GetNames(typeof(UploadFileSessionStep)).Length - 1)
            {
                await MakeFileUploadMessageAsync();
                SessionManager.DestroySession(this);
                return;
            }

            NextStep();

            await TelegramBot.WriteMessage(ChatId, CurrentStep.PromptText);
        }

        private async Task MakeFileUploadMessageAsync()
        {
            var text = "";
            text += "Titel: " + Title;


            text += "\n\n";

            text += "Beschreibung: " + Description;

            text += "\n\n";

            text += "Plattform: ";
            text = Platforms.Aggregate(text, (current, p) => current + (p + " "));

            text += "\n\n";

            text += "Bild: " + ImageLink;


            await TelegramBot.WriteMessage(GroupChatId, text);
            var file = new FileToSend(FileId);
            await TelegramBot.Bot.SendDocumentAsync(GroupChatId, file);
        }

        private void NextStep()
        {
            _sessionStep++;
        }

        private void SetTitle(Message m)
        {
            Title = m.Text;
        }

        private void UploadFile(Message m)
        {
            if (m.Document.FileId != null)
                FileId = m.Document.FileId;
            _sessionStep++;
        }

        private void SetImage(Message m)
        {
            ImageLink = m.Text;
        }

        private async void MorePlatforms(Message m)
        {
            if (m.Text != "Ja") return;
            _sessionStep = _sessionStep - 2;
            await TelegramBot.WriteMessage(m.Chat.Id, CurrentStep.PromptText);
        }

        private void SetPlatform(Message m)
        {
            Platforms.Add(m.Text);
        }

        private void SetDescription(Message m)
        {
            Description = m.Text;
        }

        private void Finalize(Message m)
        {
            if (FileId != null)
                return;

            if (m.Document.FileId != null)
                UploadFile(m);
            else if (FileId == null)
                _sessionStep--;
        }

        public class UploadStepDesciptor
        {
            public UploadFileSessionStep Step { get; set; }
            public string PromptText { get; set; }
            public MessageVerifier.MessageCondition Conditions { get; set; }
            public string InvalidMessageErrorText { get; set; }
            public Action<Message> Action { get; set; }
            public ReplyKeyboardMarkup Keyboard { get; set; }
        }
        
        public enum UploadFileSessionStep
        {
            SetTitle = 0,
            SetDescription = 1,
            SetPlatform = 2,
            MorePlatforms = 3,
            SetImage = 4,
            UploadFile = 5,
            Finalize = 6
        }

        public enum UploadMediaType
        {
            Software,
            Music,
            Video
        }
    }
}