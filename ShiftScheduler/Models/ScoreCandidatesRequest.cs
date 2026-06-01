namespace ShiftScheduler.Models
{
    public class ScoreCandidatesRequest
    {
        public string PositionId { get; set; }

        public int ShiftType { get; set; }

        public DateTime Date { get; set; }
    }
}
