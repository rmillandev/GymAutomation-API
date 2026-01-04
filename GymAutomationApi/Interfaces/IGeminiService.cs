using GymAutomationApi.Models;

namespace GymAutomationApi.Interfaces
{
    public interface IGeminiService
    {
        Task<WorkoutExtraction> ProcessedWorkout(WorkoutRequest request);
    }
}
