namespace Task7.Models;

public class BusinessTripRequest
{
    public Guid EmployeeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
