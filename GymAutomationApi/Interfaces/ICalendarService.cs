using GymAutomationApi.Models;

namespace GymAutomationApi.Interfaces
{
    public interface ICalendarService
    {
        Task<CalendarResponse> UpdateCalendarEvent(WorkoutExtraction processedData);
    }
}
