namespace ShiftScheduler.Models;

public class TrainingRow
{
    public string EmployeeId { get; set; }

    public string PositionId { get; set; }

    public int ShiftType { get; set; }

    public DateTime Date { get; set; }

    public int DayOfWeek { get; set; }

    public int Month { get; set; }

    public bool IsWeekend { get; set; }

    public int ContractHours { get; set; }

    public bool NightShift { get; set; }

    public int Label { get; set; }
}
