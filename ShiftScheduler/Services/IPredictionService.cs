using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public interface IPredictionService
    {
        IEnumerable<ScoreCandidateResponse> ScoreCandidates(ScoreCandidatesRequest request);
    }
}
