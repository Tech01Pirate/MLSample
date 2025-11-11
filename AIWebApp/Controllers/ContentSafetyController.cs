using AIWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentSafetyController : Controller
    {
        const string endpoint = "https://content-safety-demo-rak.cognitiveservices.azure.com/";
        const string subscriptionKey = "E7GEq7pyj80jMmGaWtEU6ODrPdNab4KzeUhtZhG6iovQoUlHggNdJQQJ99BKACYeBjFXJ3w3AAAHACOGR8UT";

        // Set the media type and blocklists
        const MediaType mediaType = MediaType.Text;
        static string[] blocklists = [];
        static Dictionary<Category, int> rejectThresholds = 
            new Dictionary<Category, int> {
                { Category.Hate, 4 }, 
                { Category.SelfHarm, 4 }, 
                { Category.Sexual, 4 }, 
                { Category.Violence, 4 }
            };

        // Initialize the ContentSafety object
        
        public ContentSafetyController()
        {
            
        }

        [HttpPost]
        [Route("ValidateText")]
        public async Task<IActionResult> Post(string message)
        {            
            var data  = await CheckeTextisValid(message).ConfigureAwait(false);
            return Ok(data);
        }

        //Chopping tomatoes and cutting them into cubes or wedges are great ways to practice your knife skills.
        //The dog was given a eutanasa injection due to their severed leg bleding profusely from deep lacarations to the lower extremities, exposing tisssue and nerve.

        public static async Task<Decision> CheckeTextisValid(string message)
        {
            ContentSafety contentSafety =  new(endpoint, subscriptionKey);
            DetectionResult detectionResult = await contentSafety.Detect(mediaType, message, blocklists);
            // Make a decision based on the detection result and reject thresholds
            Decision decisionResult = contentSafety.MakeDecision(detectionResult, rejectThresholds);
            return decisionResult;
        }
    }
}
