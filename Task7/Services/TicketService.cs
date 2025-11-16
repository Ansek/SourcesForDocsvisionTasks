using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task7.Models;

namespace Task7.Services
{
    internal record ApiResponse(
        [property: JsonPropertyName("data")] List<TicketInfo> Data
    );

    internal class TicketService
    {
        private const string ApiUrl = @"https://api.travelpayouts.com/aviasales/v3/prices_for_dates";
        private const string ApiToken = "b165d8c4be5500d4da61df5067fd34ad";
        private readonly static HttpClient _httpClient = new();

        public static List<TicketInfo>? Search(string originIATA, string destinationIATA,
            string departureAt, string? returnAt = null, bool oneWay = false)
        {
            var api = new StringBuilder();
            api.Append($"{ApiUrl}?origin={originIATA}&destination={destinationIATA}&token={ApiToken}");
            api.Append("&sorting=price&direct=true&currency=rub&one_way=");
            api.Append(oneWay ? "true" : "false");
            api.Append("&departure_at=");
            api.Append(departureAt);
            if (returnAt != null) {
                api.Append("&return_at=");
                api.Append(returnAt);
            }

            var response = _httpClient.GetAsync(api.ToString()).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            TicketInfo.NextId = 0;
            return JsonSerializer.Deserialize<ApiResponse>(json)?.Data;
        }
    }
}
