using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

namespace MyBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string username = null;
        List<string> mainMenuOptions = new List<string> { "Know more about the developer", "Try something smart", "Play a game" };
        List<string> microsoftCognitiveServicesMenuOptions = new List<string> { "Emotion", "Face", "Computer Vision", "Back to main menu" };
        List<string> aboutDeveloperMenuOptions = new List<string> { "education", "experiences", "friends", "sport", "dream", "weaknesses", "strengths", "website", "Back to main menu" };

        #region aboutDeveloperContent
        Dictionary<string, string> aboutDeveloperContent = new Dictionary<string, string>
        {
            {"education","Majdi is doing a Master degree in Engineering in Computer Systems, he got his Bachelor degree in 2015 from Faculty of Sciences of Monastir."},
            {"experiences","Humm, I can't tell you all Majdi's experiences but here is some of them.\nMajdi is a Microsoft Student Partner for the 3rd year, he did an internship at Microsoft Tunisia. Last summer he worked as a FullStack developer at Ernst."},
            {"friends","Majdi has a lot of friends and you are one of them because you are discussing with me :D"},
            {"sport","Majdi was Hand ball player with Sidi Bouzid Hanball Team for 5 years (2003 - 2008)!"},
            {"dream","Majdi dreams to build his own company (MacdoW) with his brothers (Chames, Dhia and Oussama)"},
            {"weaknesses","Majdi smokes a lot :( Yes this is a big problem!"},
            {"strengths","He is a dreamer, curious and always wants to learn more about new technologies. \nFor this reason he built me :D"}
        };
        #endregion

        // the hidden number for guessing number game
        int hiddenNumber;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            
            if(username != null)
            {
                PromptDialog.Choice(context, mainResumeAfter, mainMenuOptions, "How can I help you ?", "Sorry, I can't understand you!");
            }
            else
            {
                // ask the user for his name
                PromptDialog.Text(context, greetingResumeAfter, "Hi there, what's your name ?", "Sorry, I can't understand you!");
            }
        }

        private async Task greetingResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            username = await result;

            await context.PostAsync($"Nice to meet you {username}!");

            PromptDialog.Choice(context, mainResumeAfter, mainMenuOptions, "How can I help you ?", "Sorry, I can't understand you!");
        }

        private async Task mainResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            switch (await result)
            {
                case "Know more about the developer":
                    {
                        PromptDialog.Choice(context, aboutDeveloperResumeAfter, aboutDeveloperMenuOptions ,"I'm a bot developed by Majdi, so here is some information about him :", "Sorry, I can't understand you!");
                        break;
                    }
                case "Try something smart":
                    {
                        PromptDialog.Choice(context, microsoftCognitiveServicesResumeAfter, microsoftCognitiveServicesMenuOptions, "I'm trying to be more intelligent, so here is what can I do actually", "Sorry, I can't understand you!");
                        break;
                    }
                case "Play a game":
                    {
                        hiddenNumber = new Random().Next(0, 100);
                        PromptDialog.Text(context, gameResumeAfter, "I will hide a number (between 0 and 100) and you should guess the hidden number, I'll help you by telling you each time if the guessed number is greater or smaller than the hidden number\n\nLet's start, guess the hidden number (send a number 1, 2, 3...)", "Please send a valid number!");
                        break;
                    }
            }
        }

        private async Task gameResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            int number;
            if(int.TryParse(await result, out number))
            {
                if(number == hiddenNumber)
                {
                    await context.PostAsync($"Congratulations! The hidden number is {hiddenNumber}!");
                }
                else
                {
                    var response = number > hiddenNumber ? $"{number} is greater than the hidden number!" : $"{number} is smaller than the hidden number!";
                    await context.PostAsync(response);
                    PromptDialog.Text(context, gameResumeAfter, "Send me another number!", "Please send a valid number!");
                }
            }
        }

        private async Task aboutDeveloperResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            var choice = await result;

            if(aboutDeveloperContent.ContainsKey(choice))
            {
                await context.PostAsync(aboutDeveloperContent[choice]);
            }
            else
            {
                if(choice == "website")
                {
                    var reply = context.MakeMessage();
                    reply.Attachments = new List<Attachment>();

                    List<CardImage> cardImages = new List<CardImage>() { new CardImage("http://saibimajdi.com/images/shared.png") };
                    List<CardAction> cardActions = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Value = "http://saibimajdi.com",
                            Title = "Majdi SAIBI Website",
                            Type = "openUrl"
                        }
                    };

                    ThumbnailCard thumbnailCard = new ThumbnailCard()
                    {
                        Title = "Hi, I'm Majdi",
                        Subtitle = "Thinker & .NET Developer",
                        Images = cardImages,
                        Buttons = cardActions,
                    };

                    reply.Attachments.Add(thumbnailCard.ToAttachment());

                    await context.PostAsync(reply);
                }
                else
                {
                    PromptDialog.Text(context, aboutDeveloperResumeAfter, "Options :", "Sorry, I can't understand you!");
                }
            }
        }

        private async Task microsoftCognitiveServicesResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            switch (await result)
            {
                case "Emotion":
                    {
                        PromptDialog.Attachment(context, emotionResumeAfter, "Send me a picture", new List<string>{ "image/jpeg", "image/png" } ,"Sorry, I can't understand you!");
                        break;
                    }
                case "Face":
                    {
                        PromptDialog.Attachment(context, faceResumeAfter, "Send me a picture", new List<string> { "image/jpeg", "image/png" }, "Sorry, I can't understand you!");
                        break;
                    }
                case "Computer Vision":
                    {
                        PromptDialog.Attachment(context, visionResumeAfter, "Send me a picture", new List<string> { "image/jpeg", "image/png" }, "Sorry, I can't understand you!");
                        break;
                    }
            }
        }

        private async Task visionResumeAfter(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = await result as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Vision(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);
        }

        private async Task faceResumeAfter(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = await result as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Face(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);

        }

        private async Task emotionResumeAfter(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = (await result) as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Emotion(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);
        }
    }
}

/*
 
    

     */
