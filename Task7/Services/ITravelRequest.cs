using DocsVision.Platform.WebClient;
using Task7.Models;

namespace Task7.Services;

public interface ITravelRequest
{
    ManagerInfo GetManagerInfoByEmployeeId(SessionContext sessionContext, Guid employeeId);

    TravelInfo CalculateTravelInfo(SessionContext sessionContext, TravelInfoRequest request);

    void SetBusinessTripStatus(SessionContext sessionContext, BusinessTripRequest request);

    List<TicketInfo>? SearchTickets(SessionContext sessionContext, TicketInfoRequest request);

    void InitTravelRequestKind(SessionContext sessionContext, Guid cardId);
}
