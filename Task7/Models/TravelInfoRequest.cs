namespace Task7.Models;

public class TravelInfoRequest
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid CityId { get; set; }
}
