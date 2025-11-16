using System.Text.Json.Serialization;

namespace Task7.Models;

public class TicketInfo
{
    [property: JsonIgnore]
    public static int NextId { get; set; } = 0;

    [property: JsonIgnore]
    public int Id { get; set; } = NextId++;

    [property: JsonPropertyName("airline")]
    public string? DepartureAirline { get; set; }

    [property: JsonPropertyName("flight_number")]
    public string? DepartureFlightNumber { get; set; }

    [property: JsonIgnore]
    public string? ReturnAirline { get; set; }

    [property: JsonIgnore]
    public string? ReturnFlightNumber { get; set; }

    [property: JsonPropertyName("price")]
    public int Price { get; set; }

    [property: JsonPropertyName("origin_airport")]
    public string? OriginAirport { get; set; }

    [property: JsonPropertyName("destination_airport")]
    public string? DestinationAirport { get; set; }

    [property: JsonPropertyName("departure_at")]
    public string? DepartureAt { get; set; }

    [property: JsonPropertyName("return_at")]
    public string? ReturnAt { get; set; }
}
