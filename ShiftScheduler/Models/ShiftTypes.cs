namespace ShiftScheduler.Models
{
    public class ShiftTypes
    {
        public int Id { get; set; }
        public string ShiftType { get; set; }
        public string Category { get; set; }
        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
