using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Common.Contract;

namespace MyBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        readonly string emotionApiKey = System.Configuration.ConfigurationManager.AppSettings["emotionApiKey"].ToString();
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                /*ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var responseMessage = "Hi there, what's your name ?";

                //PromptDialog.Text()
                
                if (activity.Attachments?.Count > 0)
                {
                    var photoUrl = activity.Attachments[0].ContentUrl;

                    var httpClient = new HttpClient();
                    var photoStream = await httpClient.GetStreamAsync(photoUrl);

                    EmotionServiceClient emotionServiceClient = new EmotionServiceClient(emotionApiKey);

                    try
                    {
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
                }

                Activity reply = activity.CreateReply(responseMessage);

                await connector.Conversations.ReplyToActivityAsync(reply);*/

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }


    public class AI
    {
        public static string Emotion()
        {
            return "";
        }
    }
}