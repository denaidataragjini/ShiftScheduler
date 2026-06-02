using Microsoft.ML.Data;

namespace ShiftScheduler.ML;

public class EmployeePredictionInput
{
    [LoadColumn(0)]
    public string EmployeeId { get; set; }

    [LoadColumn(1)]
    public string PositionId { get; set; }

    [LoadColumn(2)]
    public float ShiftType { get; set; }

    [LoadColumn(3)]
    public float DayOfWeek { get; set; }

    [LoadColumn(4)]
    public float Month { get; set; }

    [LoadColumn(5)]
    public bool IsWeekend { get; set; }

    [LoadColumn(6)]
    public float ContractHours { get; set; }

    [LoadColumn(7)]
    public bool NightShift { get; set; }

    [LoadColumn(8)]
    [ColumnName("Label")]
    public float Label { get; set; }
}