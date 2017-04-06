using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Net.Http;

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
            
            // ask the user for his name
            PromptDialog.Text(context, greeting, "Hi there, what's your name ?", "Sorry, I can't understand you!");
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
                        PromptDialog.Text(context, emotionDelegate, "Send me a picture!", "Sorry, I can't understand you!");
                        break;
                    }
                case "Face":
                    {
                        PromptDialog.Text(context, faceDelegate, "Send me a picture!", "Sorry, I can't understand you!");
                        break;
                    }
            }
            //await context.PostAsync($"{username} , option = {await result}");
        }

        private async Task faceDelegate(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var response = await MicrosoftCognitiveServices.Face(activity.Attachments?[0].ContentUrl);

            await context.PostAsync(response);

        }

        private async Task emotionDelegate(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            var response = await MicrosoftCognitiveServices.Emotion(activity.Attachments?[0].ContentUrl);

            await context.PostAsync(response);
        }
    }

    public class MicrosoftCognitiveServices
    {
        readonly static string emotionApiKey = System.Configuration.ConfigurationManager.AppSettings["emotionApiKey"].ToString();
        readonly static string faceApiKey = System.Configuration.ConfigurationManager.AppSettings["faceApiKey"].ToString();

        public async static Task<string> Emotion(string photoUrl)
        {
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(emotionApiKey);

            var responseMessage = "";

            try
            {
                var httpClient = new HttpClient();
                var photoStream = await httpClient.GetStreamAsync(photoUrl);

                Emotion[] emotionResult = await emotionServiceClient.RecognizeAsync(photoStream);
                float happinessScore = emotionResult[0].Scores.Happiness;
                float angerScore = emotionResult[0].Scores.Anger;
                float surpriseScore = emotionResult[0].Scores.Surprise;
                float sadScore = emotionResult[0].Scores.Sadness;
                float neutralScore = emotionResult[0].Scores.Neutral;

                responseMessage = Math.Ceiling(happinessScore * 100) + "% happiness :D !\n\n";
                responseMessage += Math.Ceiling(angerScore * 100) + "% angriness >.< !\n\n";
                responseMessage += Math.Ceiling(surpriseScore * 100) + "% surprise O.O !\n\n";
                responseMessage += Math.Ceiling(sadScore * 100) + "% sadness :( !\n\n";
                responseMessage += Math.Ceiling(neutralScore * 100) + "% neutral!";
            }
            catch (Exception ex)
            {
                responseMessage = "Sorry, I can't see anything in this picture! Can you send me face picture?";
            }

            return responseMessage;
        }

        public async static Task<string> Face(string photoUrl)
        {
            FaceServiceClient faceServiceClient = new FaceServiceClient(faceApiKey);

            var responseMessage = "";

            try
            {
                var httpClient = new HttpClient();
                var photoStream = await httpClient.GetStreamAsync(photoUrl);

                var faces = await faceServiceClient.DetectAsync(photoStream);

                if(faces.Length == 0)
                {
                    responseMessage += "This picture does not contain any face!\n\n";
                }
                else
                {
                    responseMessage += $"This picture contains {faces.Length} face(s)!\n\n";
                }

                var counter = 0;

                foreach(var face in faces)
                {
                    counter++;

                    responseMessage += $"Face #{counter}: \n\n";

                    //var emotionScore = face.FaceAttributes.Emotion;
                    var age = face.FaceAttributes.Age;
                    var gender = face.FaceAttributes.Gender;

                    responseMessage += $"Gender : {gender} \n\n";
                    responseMessage += $"Age : {age} \n\n";

                }
            }
            catch(Exception ex)
            {
                return "Error while detecting faces!";
            }

            return responseMessage;
        }
        
    }
}