using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public interface IScheduleSuggestionService
    {
        IEnumerable<ScheduleSuggestionResponse> GenerateSchedule(ScheduleSuggestionRequest request);
        byte[] Generate(DateTime date);
    }
}
