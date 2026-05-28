namespace ShiftScheduler.Models;

public class ShiftCandidate
{
    public string EmployeeId { get; set; }
    public float Probability { get; set; }
    public bool PredictedLabel { get; set; }
}