namespace GymAutomationApi.Configuration
{
    public class GeminiConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.5-flash";
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
