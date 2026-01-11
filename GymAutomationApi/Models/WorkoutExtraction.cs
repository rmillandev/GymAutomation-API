using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GymAutomationApi.Models
{
    public class WorkoutExtraction
    {
        public List<ExerciseEntry> Exercises { get; set; } = new();
    }

    public class ExerciseEntry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ExerciseType Exercise { get; set; }
        public List<SetDetail> Sets { get; set; } = new();
    }

    public class SetDetail
    {
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }

}
