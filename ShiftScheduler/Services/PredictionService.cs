using Microsoft.ML;
using ShiftScheduler.ML;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly PredictionEngine<SchedulePredictionInput, SchedulePrediction> _engine;
        private readonly CsvLoaderService _csvLoaderService;
        private readonly HashSet<int> _nightShiftTypes = [18];

        public PredictionService()
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load("ML/model.zip", out _);
            _engine = mlContext.Model.CreatePredictionEngine<SchedulePredictionInput, SchedulePrediction>(model);
            _csvLoaderService = new CsvLoaderService();
        }

        public IEnumerable<ScoreCandidateResponse> ScoreCandidates(ScoreCandidatesRequest request)
        {
            var dayOfWeek = (int)request.Date.DayOfWeek;

            var month = request.Date.Month;

            var isWeekend = request.Date.DayOfWeek == DayOfWeek.Saturday || request.Date.DayOfWeek == DayOfWeek.Sunday;

            var employees = _csvLoaderService.Load<Employee>("Data/employees.csv");
            var requests = _csvLoaderService.Load<Request>("Data/requests.csv");

            // only employees unavailable on this specific date
            var unavailable = requests
                .Where(r => r.Date.Date == request.Date.Date)
                .Select(r => r.UserId)
                .ToHashSet();

            var results = new List<ScoreCandidateResponse>();

            foreach (var employee in employees)
            {
                // skip unavailable on this date
                if (unavailable.Contains(employee.Id))
                    continue;

                // skip night shift restriction
                if (_nightShiftTypes.Contains(request.ShiftType) && !employee.NightShift)
                    continue;

                var input = new SchedulePredictionInput
                {
                    EmployeeId = employee.Id,
                    PositionId = request.PositionId,
                    ShiftType = request.ShiftType,
                    DayOfWeek = dayOfWeek,
                    Month = month,
                    IsWeekend = isWeekend,
                    ContractHours = employee.ContractHours,
                    NightShift = employee.NightShift,
                    Label = 0
                };

                var prediction = _engine.Predict(input);

                results.Add(new ScoreCandidateResponse
                {
                    EmployeeId = employee.Id,
                    Employee = employee.UserName,
                    Probability = prediction.Probability,
                    PredictedLabel = prediction.PredictedLabel
                });
            }

            return results
                .OrderByDescending(x => x.Probability)
                .Take(10);
        }
    }
}