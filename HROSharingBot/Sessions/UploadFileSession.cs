using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HROSharingBot.Commands;
using HROSharingBot.Messages;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace HROSharingBot.Sessions
{
    public class UploadFileSession : Session
    {
        private int _sessionStep;
        private readonly List<UploadStepDesciptor> _steps;
        private readonly long _groupChatId = Convert.ToInt64(ConfigReader.Configuration["appSettings:GroupChatId"]);
        
        private string Title { get; set; }
        private string Description { get; set; }
        private List<string> Platforms { get; set; } = new List<string>();
        private string ImageLink { get; set; }
        private List<string> FileId { get; set; } = new List<string>();
        private UploadMediaType MediaType { get; set; }
        private string UploaderName { get; set; }
        
        public UploadStepDesciptor CurrentStep
        {
            get
            {
                var step = (UploadFileSessionStep) _sessionStep;

                return _steps.FirstOrDefault(s => s.Step == step);
            }
        }

        public UploadFileSession()
        {
            _steps = new List<UploadStepDesciptor>
            {
                new UploadStepDesciptor()
                {
                    Step = UploadFileSessionStep.SetMediaType,
                    PromptText = "Was möchtest du hochladen?",
                    Conditions = new MessageVerifier.MessageCondition()
                    {
                        Text = new MessageVerifier.TextCondition()
                        {
                            EqualTo = new[] {"Software", "Musik", "Video", "Andere"}
                        }
                    },
                    InvalidMessageErrorText = "Bitte gib eins der folgenden Paketarten an: 'Software', 'Musik', 'Video', 'Andere'",
                    Action = SetMediaType,
                    Keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                    {
                        new[]
                        {
                            new KeyboardButton("Software"),
                            new KeyboardButton("Musik"), 
                        },
                        new[]
                        {
                            new KeyboardButton("Video"),
                            new KeyboardButton("Andere"), 
                        }
                    })
                },
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
                        "Bitte gib eine der folgenden Plattformen an: Windows, Linux, OSX, Android, IOS, Andere",
                    Action = SetPlatform,
                    Keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                    {
                        new[]
                        {
                            new KeyboardButton("Windows"),
                            new KeyboardButton("Linux"), 
                        },
                        new[]
                        {
                            new KeyboardButton("OSX"),
                            new KeyboardButton("Android"), 
                        },
                        new[]
                        {
                            new KeyboardButton("IOS"),
                            new KeyboardButton("Andere"),
                        }
                        
                    })
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
                    Action = MorePlatforms,
                    Keyboard  = new ReplyKeyboardMarkup(new KeyboardButton[][]
                    {
                        new[]
                        {
                            new KeyboardButton("Ja"),
                            new KeyboardButton("Nein"), 
                        }
                    })
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
                },
                new UploadStepDesciptor()
                {
                    Step = UploadFileSessionStep.MoreFiles,
                    PromptText = "Möchtest du weitere Dateien hochladen?",
                    Conditions = new MessageVerifier.MessageCondition()
                    {
                        Text = new MessageVerifier.TextCondition {EqualTo = new[] {"Ja", "Nein"}}
                    },
                    InvalidMessageErrorText = "Bitte gib 'Ja' oder 'Nein' ein.",
                    Action = MoreFiles,
                    Keyboard  = new ReplyKeyboardMarkup(new KeyboardButton[][]
                    {
                        new[]
                        {
                            new KeyboardButton("Ja"),
                            new KeyboardButton("Nein"), 
                        }
                    })
                }
            };
        }
        
        
        

        public override async Task ExecuteMessage(Message message)
        {
            if (CommandDispatcher.ParseCommand(message.Text) != CommandDispatcher.Command.Undefined)
            {
                CommandDispatcher.RunCommand(message.Text, message.Chat.Id);
                return;
            }

            if (!message.Verify(CurrentStep.Conditions))
            {
                await TelegramBot.SendMessage(ChatId, "Ungültige Eingabe. " + CurrentStep.InvalidMessageErrorText);
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

            
            await TelegramBot.SendMessage(ChatId, CurrentStep.PromptText, CurrentStep.Keyboard);
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

            text += "Uploader: " + this.UploaderName;
            
            text += "\n\n";

            text += "Bild: " + ImageLink;

            long chatId;
            chatId = _groupChatId == 0 ? this.ChatId : _groupChatId;


            await TelegramBot.SendMessage(chatId, text);
            
            foreach (var id in FileId)
            {
                var file = new FileToSend(id);
                await TelegramBot.Bot.SendDocumentAsync(chatId, file);
            }
        }

        private void NextStep()
        {
            _sessionStep++;
        }
        
        private void SetMediaType(Message m)
        {
            this.MediaType = (UploadMediaType)Enum.Parse(typeof(UploadMediaType), m.Text, true);
        }

        private void SetTitle(Message m)
        {
            Title = m.Text;
            this.UploaderName = m.Chat.FirstName + " " + m.Chat.LastName;
        }

        private void UploadFile(Message m)
        {
            if (m.Document.FileId != null)
                FileId.Add(m.Document.FileId);
            _sessionStep++;
        }

        private void SetImage(Message m)
        {
            ImageLink = m.Text;
        }

        private async void MorePlatforms(Message m)
        {
            if (m.Text != "Ja") return;
            _sessionStep -= 2;
            await TelegramBot.SendMessage(m.Chat.Id, CurrentStep.PromptText);
        }

        private void SetPlatform(Message m)
        {
            Platforms.Add(m.Text);
        }

        private void SetDescription(Message m)
        {
            Description = m.Text;

            if (this.MediaType != UploadMediaType.Software)
                this._sessionStep += 2;
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
        
        private void MoreFiles(Message message)
        {
            if (message.Text == "Ja")
            {
                _sessionStep -= 3;
            }
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
            SetMediaType = 0,
            SetTitle = 1,
            SetDescription = 2,
            SetPlatform = 3,
            MorePlatforms = 4,
            SetImage = 5,
            UploadFile = 6,
            Finalize = 7,
            MoreFiles = 8
        }

        private enum UploadMediaType
        {
            Software,
            Musik,
            Video,
            Andere
        }
    }
}