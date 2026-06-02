namespace ShiftScheduler.Models
{
    public class ShiftSuggestionResponse
    {
        public int ShiftType { get; set; }
        public string Category { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public float Probability { get; set; }
    }
}
