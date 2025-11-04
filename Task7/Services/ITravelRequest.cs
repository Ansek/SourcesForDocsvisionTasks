using DocsVision.Platform.WebClient;
using Task7.Models;

namespace Task7.Services;

public interface ITravelRequest
{
    public ManagerInfo GetManagerInfoByEmployeeId(SessionContext sessionContext, Guid employeeId);

    public TravelInfo CalculateTravelInfo(SessionContext sessionContext, TravelInfoRequest request);

    void InitTravelRequestKind(SessionContext sessionContext, Guid cardId);
}
