using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using System.Net.Http;
using System.Linq;

namespace MyBot
{

    public class MicrosoftCognitiveServices
    {
        readonly static string emotionAPIKey = System.Configuration.ConfigurationManager.AppSettings["emotionApiKey"].ToString();
        readonly static string faceAPIKey = System.Configuration.ConfigurationManager.AppSettings["faceApiKey"].ToString();
        readonly static string visionAPIKey = System.Configuration.ConfigurationManager.AppSettings["visionApiKey"].ToString();

        public async static Task<string> Emotion(string photoUrl)
        {
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(emotionAPIKey);

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
            // call the Face API
            FaceServiceClient faceServiceClient = new FaceServiceClient(faceAPIKey);

            var responseMessage = "";

            try
            {
                // get the file stream through the picture path
                var httpClient = new HttpClient();
                var photoStream = await httpClient.GetStreamAsync(photoUrl);

                // preparing the face attributes type
                var faceAttributes = new FaceAttributeType[]
                {
                    FaceAttributeType.Age,
                    FaceAttributeType.Gender
                };

                // get detected faces
                var faces = await faceServiceClient.DetectAsync(photoStream,false, true, faceAttributes);

                if(faces.Length == 0)
                {
                    // can't detect any face in this picture
                    responseMessage += "This picture does not contain any face!\n\n";
                }
                else
                {
                    // display number of faces detected
                    responseMessage += $"This picture contains {faces.Length} face(s)!\n\n";
                }

                var counter = 0;

                foreach(var face in faces)
                {
                    counter++;
                    responseMessage += $"Face #{counter}: \n\n";

                    // get the age 
                    var age = face.FaceAttributes.Age;
                    // get the gender
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

        public async static Task<string> Vision(string photoUrl)
        {
            // initialize the Vision Service Client
            VisionServiceClient visionServiceClient = new VisionServiceClient(visionAPIKey);

            var responseMessage = "";

            try
            {
                // get file stream through the picture url
                var httpClient = new HttpClient();
                var photoStream = await httpClient.GetStreamAsync(photoUrl);

                var visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description };

                // get description from picture
                var result = await visionServiceClient.AnalyzeImageAsync(photoStream, visualFeatures);

                // get picture captions
                var captions = string.Join(",", result.Description.Captions.Select(c => c.Text));

                // get picture tags
                var tags = string.Join(",", result.Description.Tags.Take(10));

                // get dominate colors
                var colors = string.Join(",", result.Color.DominantColors);

                // prepare response message
                responseMessage += result.Categories.Count() > 0 ? $"Categorie: {result.Categories.FirstOrDefault().Name}\n\n" : "N/A\n\n"; ;
                responseMessage += $"Is adult content: {result.Adult.IsAdultContent}\n\n";
                responseMessage += $"Captions: {captions}\n\n";
                responseMessage += $"Tags: {tags}\n\n";
                responseMessage += $"Dominate color: {colors}\n\n";
            }
            catch(Exception ex)
            {
                return "Can't find any thing in this picture!";
            }

            return responseMessage;
        }
        
    }
}