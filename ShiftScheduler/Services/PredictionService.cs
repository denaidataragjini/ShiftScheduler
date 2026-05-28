using Microsoft.ML;
using ShiftScheduler.ML;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public class PredictionService
    {
        private readonly PredictionEngine<SchedulePredictionInput, SchedulePrediction> _engine;
        private readonly CsvLoaderService _csvLoaderService;
        private readonly HashSet<int> _nightShiftTypes = new() { 18 };

        public PredictionService(string modelPath)
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(modelPath, out _);
            _engine = mlContext.Model.CreatePredictionEngine<SchedulePredictionInput, SchedulePrediction>(model);
            _csvLoaderService = new CsvLoaderService();
        }

        public List<ShiftCandidate> ScoreCandidates(
            string positionId,
            int shiftType,
            int dayOfWeek,
            int month,
            bool isWeekend,
            DateTime date)
        {
            var employees = _csvLoaderService.Load<Employee>("Data/employees.csv");
            var requests = _csvLoaderService.Load<Request>("Data/requests.csv");

            // only employees unavailable on this specific date
            var unavailable = requests
                .Where(r => r.Date.Date == date.Date)
                .Select(r => r.UserId)
                .ToHashSet();

            var results = new List<ShiftCandidate>();

            foreach (var employee in employees)
            {
                // skip unavailable on this date
                if (unavailable.Contains(employee.Id))
                    continue;

                // skip night shift restriction
                if (_nightShiftTypes.Contains(shiftType) && !employee.NightShift)
                    continue;

                var input = new SchedulePredictionInput
                {
                    EmployeeId = employee.Id,
                    PositionId = positionId,
                    ShiftType = shiftType,
                    DayOfWeek = dayOfWeek,
                    Month = month,
                    IsWeekend = isWeekend,
                    ContractHours = employee.ContractHours,
                    NightShift = employee.NightShift,
                    Label = 0
                };

                var prediction = _engine.Predict(input);

                results.Add(new ShiftCandidate
                {
                    EmployeeId = employee.Id,
                    Probability = prediction.Probability,
                    PredictedLabel = prediction.PredictedLabel
                });
            }

            return results
                .OrderByDescending(x => x.Probability)
                .ToList();
        }
    }
}