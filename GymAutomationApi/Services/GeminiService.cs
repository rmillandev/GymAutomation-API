using GymAutomationApi.Configuration;
using GymAutomationApi.Interfaces;
using GymAutomationApi.Models;
using Microsoft.Extensions.Options;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using Newtonsoft.Json;

namespace GymAutomationApi.Services
{
    public class GeminiService : IGeminiService
    {

        private readonly GeminiConfig geminiConfig;
        private readonly GenerativeModel generativeModel;

        public GeminiService(IOptions<GeminiConfig> config)
        {
            geminiConfig = config.Value;

            var googleAI = new GoogleAI(geminiConfig.ApiKey);
            generativeModel = googleAI.GenerativeModel(Model.Gemini25Flash);
        }

        public async Task<WorkoutExtraction> ProcessedWorkout(WorkoutRequest request)
        {
            try
            {
                string systemPrompt = @"
                You are a gym assistant. Your task is to extract workout data from a user message.
                Respond ONLY with a JSON object in this exact format:
                {
                  ""exercise"": ""string"",
                  ""weight"": double,
                  ""reps"": integer,
                  ""sets"": integer
                }
                If any value is missing, use 0. Do not include markdown formatting or extra text.";

                var response = await generativeModel.GenerateContent($"{systemPrompt} \n User message: {request.RawMessage}");

                var jsonRaw = response.Text?.Replace("```json", "").Replace("```", "").Trim() ?? "{}";

                var result = JsonConvert.DeserializeObject<WorkoutExtraction>(jsonRaw);
                return result ?? new WorkoutExtraction();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
