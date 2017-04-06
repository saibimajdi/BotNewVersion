using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace MyBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string username = null;
        List<string> mainMenuOptions = new List<string> { "Emotion", "Face", "Option 3" };
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            
            PromptDialog.Text(context, greeting, "Hi there, what's your name ?", "Sorry, I can't understand you!");

            // return our reply to the user
            //await context.PostAsync(responseText);
            //context.Wait(MessageReceivedAsync);
        }

        private async Task greeting(IDialogContext context, IAwaitable<string> result)
        {
            username = await result;
            var responseText = $"Nice to meet you {username}!\n\n";

            PromptDialog.Choice(context, mainMenu, mainMenuOptions, "I can help you with one of this options:", "Sorry, I can't understand you!");
        }

        private async Task mainMenu(IDialogContext context, IAwaitable<string> result)
        {
            switch(await result)
            {
                case "Emotion":
                    {
                        break;
                    }
            }
            await context.PostAsync($"{username} , option = {await result}");
        }
    }
}