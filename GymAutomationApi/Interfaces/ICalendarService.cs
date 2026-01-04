namespace GymAutomationApi.Interfaces
{
    public interface ICalendarService
    {
        Task<bool> UpdateCalendarEvent(string processedData);
    }
}
