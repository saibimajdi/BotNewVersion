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
        List<string> mainMenuOptions = new List<string> { "Emotion", "Face", "Computer Vision" };
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
                PromptDialog.Choice(context, mainMenu, mainMenuOptions, "I can help you with one of this options:", "Sorry, I can't understand you!");
            }
            else
            {
                // ask the user for his name
                PromptDialog.Text(context, greeting, "Hi there, what's your name ?", "Sorry, I can't understand you!");
            }
        }

        private async Task greeting(IDialogContext context, IAwaitable<string> result)
        {
            username = await result;

            await context.PostAsync($"Nice to meet you {username}!");

            PromptDialog.Choice(context, mainMenu, mainMenuOptions, "I can help you with one of this options:", "Sorry, I can't understand you!");
        }

        private async Task mainMenu(IDialogContext context, IAwaitable<string> result)
        {
            switch (await result)
            {
                case "Emotion":
                    {
                        PromptDialog.Attachment(context, emotionDelegate, "Send me a picture", new List<string>{ "image/jpeg", "image/png" } ,"Sorry, I can't understand you!");
                        break;
                    }
                case "Face":
                    {
                        PromptDialog.Attachment(context, faceDelegate, "Send me a picture", new List<string> { "image/jpeg", "image/png" }, "Sorry, I can't understand you!");
                        break;
                    }
                case "Computer Vision":
                    {
                        PromptDialog.Attachment(context, visionDelegate, "Send me a picture", new List<string> { "image/jpeg", "image/png" }, "Sorry, I can't understand you!");
                        break;
                    }
            }
        }

        private async Task visionDelegate(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = await result as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Vision(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);
        }

        private async Task faceDelegate(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = await result as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Face(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);

        }

        private async Task emotionDelegate(IDialogContext context, IAwaitable<IEnumerable<Attachment>> result)
        {
            var activity = (await result) as IEnumerable<Attachment>;

            var response = await MicrosoftCognitiveServices.Emotion(activity.FirstOrDefault().ContentUrl);

            await context.PostAsync(response);
        }
    }
}