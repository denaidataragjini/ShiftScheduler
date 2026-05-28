namespace ShiftScheduler.Services;

using CsvHelper;
using ShiftScheduler.Models;
using System.Globalization;

public class TrainingDataGeneratorService
{
    private const int NegativesPerPositive = 1;

    private readonly List<int> NightShiftTypes = new() { 18 };

    public List<TrainingRow> Generate(
        List<Schedule> schedules,
        List<Employee> employees,
        List<Request> requests)
    {
        var result = new List<TrainingRow>();

        var random = new Random(42);

        var scheduleLookup = schedules
            .Select(x => $"{x.EmployeeId}_{x.Date.Date}_{x.ShiftType}")
            .ToHashSet();

        var requestLookup = requests
            .Select(x => $"{x.UserId}_{x.Date.Date}")
            .ToHashSet();

        foreach (var schedule in schedules)
        {
            var assignedEmployee = employees
                .FirstOrDefault(x => x.Id == schedule.EmployeeId);

            if (assignedEmployee == null)
                continue;

            // POSITIVE
            result.Add(new TrainingRow
            {
                EmployeeId = schedule.EmployeeId,
                PositionId = schedule.PositionId,
                ShiftType = schedule.ShiftType,
                DayOfWeek = schedule.DayOfWeek,
                Month = schedule.Month,
                IsWeekend = schedule.IsWeekend,
                ContractHours = assignedEmployee.ContractHours,
                NightShift = assignedEmployee.NightShift,
                Label = 1
            });

            // NEGATIVE CANDIDATES
            var candidates = employees
                .Where(e => e.Id != schedule.EmployeeId)

                // already working same shift/date
                .Where(e => !scheduleLookup.Contains(
                    $"{e.Id}_{schedule.Date.Date}_{schedule.ShiftType}"))

                // has request/unavailability
                .Where(e => !requestLookup.Contains(
                    $"{e.Id}_{schedule.Date.Date}"))

                // night restriction
                .Where(e =>
                    !NightShiftTypes.Contains(schedule.ShiftType)
                    || e.NightShift)

                .OrderBy(x => random.Next())
                .Take(NegativesPerPositive)
                .ToList();

            // NEGATIVES
            foreach (var candidate in candidates)
            {
                result.Add(new TrainingRow
                {
                    EmployeeId = candidate.Id,
                    PositionId = schedule.PositionId,
                    ShiftType = schedule.ShiftType,
                    DayOfWeek = schedule.DayOfWeek,
                    Month = schedule.Month,
                    IsWeekend = schedule.IsWeekend,
                    ContractHours = candidate.ContractHours,
                    NightShift = candidate.NightShift,
                    Label = 0
                });
            }
        }

        return result;
    }
    public void ExportCsv(List<TrainingRow> rows, string path)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(rows);
    }
}