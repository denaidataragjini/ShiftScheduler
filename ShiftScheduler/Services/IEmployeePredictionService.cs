using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public interface IEmployeePredictionService
    {
        IEnumerable<ScoreCandidateResponse> ScoreCandidates(ScoreCandidatesRequest request);
    }
}
