namespace Task7.Models;

public class TicketInfoRequest
{
    public Guid DestinationId { get; set; }
    public DateTime DepartureAt { get; set; }
    public DateTime ReturnAt { get; set; }
}