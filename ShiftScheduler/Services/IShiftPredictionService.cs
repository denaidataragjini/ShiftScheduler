using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public interface IShiftPredictionService
    {
        IEnumerable<ShiftSuggestionResponse> PredictShifts(ShiftSuggestionRequest request);
    }
}
