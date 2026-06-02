namespace ShiftScheduler.Models
{
    public class ScheduleSuggestionResponse
    {
        public string PositionId { get; set; }
        public string PositionName { get; set; }


        public int ShiftType { get; set; }


        public string Category { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Employee { get; set; }

        public float Score { get; set; }
    }
}
