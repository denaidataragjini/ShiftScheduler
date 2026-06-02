using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ShiftScheduler.Models;

namespace ShiftScheduler.Services
{
    public class ScheduleSuggestionService(IShiftPredictionService shiftPredictionService, IEmployeePredictionService employeePredictionService) : IScheduleSuggestionService
    {
        private readonly IShiftPredictionService _shiftPredictionService = shiftPredictionService;
        private readonly IEmployeePredictionService _employeePredictionService = employeePredictionService;
        private readonly CsvLoaderService _csvLoaderService = new();

        public IEnumerable<ScheduleSuggestionResponse> GenerateSchedule(ScheduleSuggestionRequest request)
        {

            var predictedShifts = _shiftPredictionService.PredictShifts(
                new ShiftSuggestionRequest
                {
                    PositionId = request.PositionId,
                    Date = request.Date
                }
            );

            var assignedEmployees = new HashSet<string>();

            var result = new List<ScheduleSuggestionResponse>();
            var positions = _csvLoaderService.Load<Position>("Data/positions.csv");

            foreach (var shift in predictedShifts)
            {
                var candidates = _employeePredictionService.ScoreCandidates(
                new ScoreCandidatesRequest
                {
                    PositionId = request.PositionId,
                    ShiftType = shift.ShiftType,
                    Date = request.Date
                });
                var bestCandidate = candidates.FirstOrDefault(x => !assignedEmployees.Contains(x.EmployeeId));

                if (bestCandidate == null)
                    continue;

                var position = positions.FirstOrDefault(x => x.Id == request.PositionId);

                assignedEmployees.Add(bestCandidate.EmployeeId);

                result.Add(new ScheduleSuggestionResponse
                {
                    PositionId = request.PositionId,
                    PositionName= position?.Name ?? request.PositionId,
                    ShiftType = shift.ShiftType,
                    Category = shift.Category,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    Employee = bestCandidate.Employee,
                    Score = bestCandidate.Probability,

                });
            }

            return result;
        }

        public IEnumerable<ScheduleSuggestionResponse> GenerateDailySchedule(DateTime date)
        {
            var result = new List<ScheduleSuggestionResponse>();

            var positions = _csvLoaderService.Load<Position>("Data/positions.csv");

            foreach (var position in positions)
            {
                var schedule = GenerateSchedule(
                    new ScheduleSuggestionRequest
                    {
                        PositionId = position.Id,
                        Date = date
                    });

                result.AddRange(schedule);
            }

            return result;
        }


        public byte[] Generate(DateTime date)
        {
            var suggestions = GenerateDailySchedule(date);

            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"AI GENERATED SCHEDULE - {date:dd/MM/yyyy}")
                        .FontSize(20)
                        .Bold();

                    page.Content()
                        .Column(column =>
                        {
                            foreach (var positionGroup in suggestions
                                         .GroupBy(x => x.PositionId))
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Border(1)
                                    .Padding(10)
                                    .Column(card =>
                                    {
                                        card.Item()
                                            .Text($"POSITION {positionGroup.First().PositionName}")
                                            .FontSize(16)
                                            .Bold();

                                        card.Item()
                                            .PaddingVertical(5);

                                        foreach (var shift in positionGroup
                                                     .OrderBy(x => x.StartTime))
                                        {
                                            card.Item()
                                                .PaddingTop(10);

                                            card.Item()
                                                .Text(GetCategoryTitle(
                                                    shift.Category))
                                                .FontSize(13)
                                                .Bold();

                                            card.Item()
                                                .Text(
                                                    $"{shift.StartTime} - {shift.EndTime}");

                                            card.Item()
                                                .Text(
                                                    shift.Employee);
                                        }
                                    });
                            }
                        });
                });
            }).GeneratePdf();
        }
        private static string GetCategoryTitle(string category)
        {
            return category switch
            {
                "Mattina" => "☀ MATTINA",
                "Pomeriggio" => "🌇 POMERIGGIO",
                "Sera" => "🌙 SERA",
                "Notte" => "🌑 NOTTE",
                "Spezzato" => "🔄 SPEZZATO",
                _ => category.ToUpper()
            };
        }
    }

}
