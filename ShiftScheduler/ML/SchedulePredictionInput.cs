namespace ShiftScheduler.ML;

public class SchedulePredictionInput
{
    public string EmployeeId { get; set; }

    public string PositionId { get; set; }

    public float ShiftType { get; set; }

    public float DayOfWeek { get; set; }

    public bool IsWeekend { get; set; }

    public float ContractHours { get; set; }

    public bool NightShift { get; set; }

    public bool Label { get; set; }
}