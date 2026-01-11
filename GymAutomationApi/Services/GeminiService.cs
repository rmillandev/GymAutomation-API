using GymAutomationApi.Configuration;
using GymAutomationApi.Interfaces;
using GymAutomationApi.Models;
using GymAutomationApi.Utils.Exceptions;
using Microsoft.Extensions.Options;
using Mscc.GenerativeAI;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GymAutomationApi.Services
{
    public class GeminiService : IGeminiService
    {

        private readonly GeminiConfig geminiConfig;
        private readonly GenerativeModel generativeModel;
        private readonly ILogger<GeminiService> logger;

        public GeminiService(IOptions<GeminiConfig> config, ILogger<GeminiService> logger)
        {
            geminiConfig = config.Value;

            var googleAI = new GoogleAI(geminiConfig.ApiKey);
            generativeModel = googleAI.GenerativeModel(geminiConfig.Model);

            this.logger = logger;
        }

        public async Task<WorkoutExtraction> ProcessedWorkout(WorkoutRequest request)
        {
            logger.LogInformation($"Processing workout request: {request.RawMessage}");

            if (string.IsNullOrEmpty(request.RawMessage)) throw new BadRequestException("The message cannot empty.");

            string systemPrompt = @"You are a specialized fitness data extractor. Your ONLY job is to convert user messages into a specific JSON format.

ALLOWED EXERCISES:
[BenchPress, InclineBenchPress, DeclineBenchPress, DumbbellBenchPress, ChestFly, CableFly, PushUp, PullUp, ChinUp, LatPulldown, BarbellRow, DumbbellRow, SeatedRow, Deadlift, Squat, FrontSquat, LegPress, Lunges, LegExtension, LegCurl, CalfRaise, ShoulderPress, ArnoldPress, LateralRaise, FrontRaise, RearDeltFly, UprightRow, BicepCurl, HammerCurl, ConcentrationCurl, TricepPushdown, SkullCrusher, Dips, Plank, Crunch, LegRaise, RussianTwist]

STRICT RULES:
1. OUTPUT ONLY VALID JSON. No conversational text.
2. The ""exercise"" field MUST be a STRING from the list above. Example: ""Squat"".
3. NEVER use integers or indexes for the ""exercise"" name.
4. If an exercise is not in the list, use ""Unknown"".
5. Use null for weight if not specified.
6. The JSON structure must start directly with the ""exercises"" key.

OUTPUT FORMAT (STRICT):
{
  ""exercises"": [
    {
      ""exercise"": ""ExactNameFromList"",
      ""sets"": [
        {
          ""setNumber"": 1,
          ""reps"": 10,
          ""weight"": 80.5
        }
      ]
    }
  ]
}

EXAMPLE:
User: ""Hice 3 series de sentadillas, la primera 10 reps con 100kg, luego 8 con 100 y la ultima 6 con 90""
Response:
{
  ""exercises"": [
    {
      ""exercise"": ""squat"",
      ""sets"": [
        { ""setNumber"": 1, ""reps"": 10, ""weight"": 100 },
        { ""setNumber"": 2, ""reps"": 8, ""weight"": 100 },
        { ""setNumber"": 3, ""reps"": 6, ""weight"": 90 }
      ]
    }
  ]
}";

            var response = await generativeModel.GenerateContent($"{systemPrompt} \n User message: {request.RawMessage}");

            var jsonRaw = response.Text?
                .Replace("```json", "")
                .Replace("```", "")
                .Trim() ?? "{}";

            if (string.IsNullOrWhiteSpace(jsonRaw))
            {
                logger.LogWarning("Empty response from Gemini. TraceId: {TraceId}", Activity.Current?.Id ?? "N/A");
                throw new ResponseParsingException("Failed to extract workout details from the response.");
            }

            try
            {

                var result = JsonConvert.DeserializeObject<WorkoutExtraction>(jsonRaw);

                if (result == null)
                {
                    logger.LogWarning("Deserialization returned null. RawResponseSnippet: {Snippet}",
                        jsonRaw.Length > 300 ? jsonRaw[..300] : jsonRaw
                    );
                    throw new ResponseParsingException("Failed to extract workout details from the response.");
                }

                return result ?? new WorkoutExtraction();

            } catch (JsonException jsonEx)
            {
                logger.LogError(jsonEx,
                    "JSON parsing failed. Provider: Gemini. RawResponseSnippet: {Snippet}",
                    jsonRaw.Length > 300 ? jsonRaw[..300] : jsonRaw
                );
                throw new ResponseParsingException("Failed to extract workout details from the response.");
            }
           
        }
    }
}
