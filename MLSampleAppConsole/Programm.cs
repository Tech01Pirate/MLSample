using MLSampleAppConsole.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MLSampleAppConsole
{
    public class Programm
    { // Replace the placeholders with your own values
        const string endpoint = "";
        const string subscriptionKey = "";

        // Set the media type and blocklists
        const MediaType mediaType = MediaType.Text;
        static string[] blocklists = [];
        static Dictionary<Category, int> rejectThresholds = new Dictionary<Category, int> {
                { Category.Hate, 4 }, { Category.SelfHarm, 4 }, { Category.Sexual, 4 }, { Category.Violence, 4 }
            };
        static async Task Main(string[] args)
        {
            // Initialize the ContentSafety object
            ContentSafety contentSafety = new ContentSafety(endpoint, subscriptionKey);
            await CheckeTextisValid(contentSafety).ConfigureAwait(false);
        }

        public static async Task CheckeTextisValid(ContentSafety contentSafety)
        {
            Console.WriteLine("\n---------------------------------");
            Console.WriteLine("Enter the content to be Tested.");

            // Set the content to be tested
            string content = Console.ReadLine(); 

            // Detect content safety
            DetectionResult detectionResult = await contentSafety.Detect(mediaType, content, blocklists);

            // Set the reject thresholds for each category


            // Make a decision based on the detection result and reject thresholds
            Decision decisionResult = contentSafety.MakeDecision(detectionResult, rejectThresholds);

            while (true)
            {
                await CheckeTextisValid(contentSafety).ConfigureAwait(false);
            }
        }
    }
    public class ContentSafety
    {
        public string Endpoint { get; set; }
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// The version of the Content Safety API to use.
        /// </summary>
        public static readonly string API_VERSION = "2024-09-01";

        /// <summary>
        /// The valid threshold values.
        /// </summary>
        public static readonly int[] VALID_THRESHOLD_VALUES = { -1, 0, 2, 4, 6 };

        /// <summary>
        /// The HTTP client.
        /// </summary>
        public static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// The JSON serializer options.
        /// </summary>
        public static readonly JsonSerializerOptions options =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSafety"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint URL for the Content Safety API.</param>
        /// <param name="subscriptionKey">The subscription key for the Content Safety API.</param>
        public ContentSafety(string endpoint, string subscriptionKey)
        {
            Endpoint = endpoint;
            SubscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// Builds the URL for the Content Safety API based on the media type.
        /// </summary>
        /// <param name="mediaType">The type of media to analyze.</param>
        /// <returns>The URL for the Content Safety API.</returns>
        public string BuildUrl(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Text:
                    return $"{Endpoint}/contentsafety/text:analyze?api-version={API_VERSION}";
                case MediaType.Image:
                    return $"{Endpoint}/contentsafety/image:analyze?api-version={API_VERSION}";
                default:
                    throw new ArgumentException($"Invalid Media Type {mediaType}");
            }
        }

        /// <summary>
        /// Builds the request body for the Content Safety API request.
        /// </summary>
        /// <param name="mediaType">The type of media to analyze.</param>
        /// <param name="content">The content to analyze.</param>
        /// <param name="blocklists">The blocklists to use for text analysis.</param>
        /// <returns>The request body for the Content Safety API request.</returns>
        public DetectionRequest BuildRequestBody(MediaType mediaType, string content, string[] blocklists)
        {
            switch (mediaType)
            {
                case MediaType.Text:
                    return new TextDetectionRequest(content, blocklists);
                case MediaType.Image:
                    return new ImageDetectionRequest(content);
                default:
                    throw new ArgumentException($"Invalid Media Type {mediaType}");
            }
        }

        /// <summary>
        /// Deserializes the JSON string into a DetectionResult object based on the media type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="mediaType">The media type of the detection result.</param>
        /// <returns>The deserialized DetectionResult object for the Content Safety API response.</returns>
        public DetectionResult? DeserializeDetectionResult(string json, MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.Text:
                    return JsonSerializer.Deserialize<TextDetectionResult>(json, options);
                case MediaType.Image:
                    return JsonSerializer.Deserialize<ImageDetectionResult>(json, options);
                default:
                    throw new ArgumentException($"Invalid Media Type {mediaType}");
            }
        }

        /// <summary>
        /// Detects unsafe content using the Content Safety API.
        /// </summary>
        /// <param name="mediaType">The media type of the content to detect.</param>
        /// <param name="content">The content to detect.</param>
        /// <param name="blocklists">The blocklists to use for text detection.</param>
        /// <returns>The response from the Content Safety API.</returns>
        public async Task<DetectionResult> Detect(MediaType mediaType, string content, string[] blocklists)
        {
            string url = BuildUrl(mediaType);
            DetectionRequest requestBody = BuildRequestBody(mediaType, content, blocklists);
            string payload = JsonSerializer.Serialize(requestBody, requestBody.GetType(), options);

            var msg = new HttpRequestMessage(HttpMethod.Post, url);
            msg.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            msg.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            HttpResponseMessage response = await client.SendAsync(msg);
            string responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine((int)response.StatusCode);
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            Console.WriteLine(responseText);

            if (!response.IsSuccessStatusCode)
            {
                DetectionErrorResponse? error =
                    JsonSerializer.Deserialize<DetectionErrorResponse>(responseText, options);
                if (error == null || error.error == null || error.error.code == null || error.error.message == null)
                {
                    throw new DetectionException(response.StatusCode.ToString(),
                                                 $"Error is null. Response text is {responseText}");
                }
                throw new DetectionException(error.error.code, error.error.message);
            }

            DetectionResult? result = DeserializeDetectionResult(responseText, mediaType);
            if (result == null)
            {
                throw new DetectionException(response.StatusCode.ToString(),
                                             $"HttpResponse is null. Response text is {responseText}");
            }

            return result;
        }

        /// <summary>
        /// Gets the severity score of the specified category from the given detection result.
        /// </summary>
        /// <param name="category">The category to get the severity score for.</param>
        /// <param name="detectionResult">The detection result object to retrieve the severity score from.</param>
        /// <returns>The severity score of the specified category.</returns>
        public int? GetDetectionResultByCategory(Category category, DetectionResult detectionResult)
        {
            int? severityResult = null;
            if (detectionResult.CategoriesAnalysis != null)
            {
                foreach (var detailedResult in detectionResult.CategoriesAnalysis)
                {
                    if (detailedResult.Category == category.ToString())
                    {
                        severityResult = detailedResult.Severity;
                    }
                }
            }

            return severityResult;
        }

        /// <summary>
        /// Makes a decision based on the detection result and the specified reject thresholds.
        /// Users can customize their decision-making method.
        /// </summary>
        /// <param name="detectionResult">The detection result object to make the decision on.</param>
        /// <param name="rejectThresholds">The reject thresholds for each category.</param>
        /// <returns>The decision made based on the detection result and the specified reject thresholds.</returns>
        public Decision MakeDecision(DetectionResult detectionResult, Dictionary<Category, int> rejectThresholds)
        {
            Dictionary<Category, Action> actionResult = new Dictionary<Category, Action>();
            Action finalAction = Action.Accept;
            foreach (KeyValuePair<Category, int> pair in rejectThresholds)
            {
                if (!VALID_THRESHOLD_VALUES.Contains(pair.Value))
                {
                    throw new ArgumentException("RejectThreshold can only be in (-1, 0, 2, 4, 6)");
                }

                int? severity = GetDetectionResultByCategory(pair.Key, detectionResult);
                if (severity == null)
                {
                    throw new ArgumentException($"Can not find detection result for {pair.Key}");
                }

                Action action;
                if (pair.Value != -1 && severity >= pair.Value)
                {
                    action = Action.Reject;
                }
                else
                {
                    action = Action.Accept;
                }
                actionResult[pair.Key] = action;

                if (action.CompareTo(finalAction) > 0)
                {
                    finalAction = action;
                }
            }

            // blocklists
            if (detectionResult is TextDetectionResult textDetectionResult)
            {
                if (textDetectionResult.BlocklistsMatch != null &&
                    textDetectionResult.BlocklistsMatch.Count > 0)
                {
                    finalAction = Action.Reject;
                }
            }

            Console.WriteLine(finalAction);
            foreach (var res in actionResult)
            {
                Console.WriteLine($"Category: {res.Key}, Action: {res.Value}");
            }

            return new Decision(finalAction, actionResult);
        }
    }
}
