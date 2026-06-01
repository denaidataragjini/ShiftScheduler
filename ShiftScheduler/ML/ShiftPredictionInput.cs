using Microsoft.ML.Data;

namespace ShiftScheduler.ML
{
    public class ShiftPredictionInput
    {
        [LoadColumn(0)]
        public string PositionId { get; set; }
        [LoadColumn(1)]
        public float ShiftType { get; set; }
        [LoadColumn(2)]
        public float DayOfWeek { get; set; }
        [LoadColumn(3)]
        public float Month { get; set; }
        [LoadColumn(4)]
        public bool IsWeekend { get; set; }

        [LoadColumn(5)]
        [ColumnName("Label")]
        public bool Label { get; set; }
    }
}
