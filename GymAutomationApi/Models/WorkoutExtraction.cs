namespace GymAutomationApi.Models
{
    public class WorkoutExtraction
    {
        public string Exercise { get; set; } = string.Empty;
        public double Weight { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
    }
}
