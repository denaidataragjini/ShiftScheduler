namespace ShiftScheduler.Models;

public class Employee
{
    public string Id { get; set; }
    public string UserName { get; set; }

    public bool NightShift { get; set; }

    public int ContractHours { get; set; }

}
