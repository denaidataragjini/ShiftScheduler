namespace ShiftScheduler.Models;

public class ScoreCandidateResponse
{
    public string Employee { get; set; }
    public string EmployeeId { get; set; }
    public float Probability { get; set; }
    public bool PredictedLabel { get; set; }
}