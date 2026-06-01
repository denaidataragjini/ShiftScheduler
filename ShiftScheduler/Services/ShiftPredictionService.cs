using Microsoft.ML;
using ShiftScheduler.ML;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public class ShiftPredictionService : IShiftPredictionService
    {
        private readonly PredictionEngine<ShiftPredictionInput, ShiftPrediction> _engine;
        private readonly CsvLoaderService _csvLoaderService;
        public ShiftPredictionService()
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load("ML/shift_model.zip", out _);
            _engine = mlContext.Model.CreatePredictionEngine<ShiftPredictionInput, ShiftPrediction>(model);
            _csvLoaderService = new CsvLoaderService();
        }

        public IEnumerable<ShiftSuggestionResponse> PredictShifts(ShiftSuggestionRequest request)
        {
            var schedules = _csvLoaderService.Load<Schedule>("Data/schedules.csv");

            var shiftTypes = _csvLoaderService.Load<ShiftTypes>("Data/shift_types.csv");

            var dayOfWeek = (int)request.Date.DayOfWeek;

            var month = request.Date.Month;

            var isWeekend = request.Date.DayOfWeek == DayOfWeek.Saturday || request.Date.DayOfWeek == DayOfWeek.Sunday;


            //TODO: This one might be changed as idea, just as a number of shifts that model can predict....
            var averageShiftCount = schedules
                .Where(x => x.PositionId == request.PositionId)
                .Where(x => x.DayOfWeek == dayOfWeek)
                .GroupBy(x => x.Date.Date)
                .Select(g => g.Select(x => x.ShiftType)
                     .Distinct()
                     .Count())
                .DefaultIfEmpty(1)
                .Average();

            var expectedCount = Math.Max(1, (int)Math.Round(averageShiftCount));

            var predictions = new List<ShiftSuggestionResponse>();

            foreach (var shift in shiftTypes)
            {
                var input =
                    new ShiftPredictionInput
                    {
                        PositionId = request.PositionId,
                        DayOfWeek = dayOfWeek,
                        Month = month,
                        IsWeekend = isWeekend,
                        ShiftType = shift.Id
                    };

                var prediction = _engine.Predict(input);

                predictions.Add(new ShiftSuggestionResponse
                {
                    ShiftType = shift.Id,

                    Probability =
                            prediction.Probability
                });
            }

            return predictions
                .OrderByDescending(x => x.Probability)
                .Take(expectedCount)
                .ToList();
        }
    }
}