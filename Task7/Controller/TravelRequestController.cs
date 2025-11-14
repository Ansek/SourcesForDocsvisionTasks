using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;
using Microsoft.AspNetCore.Mvc;
using Task7.Models;
using Task7.Services;

namespace Task7.Controller;

public class TravelRequestController: ControllerBase
{
    private readonly ICurrentObjectContextProvider _contextProvider;
    private readonly ITravelRequest _travelRequestService;

    public TravelRequestController(
        ICurrentObjectContextProvider contextProvider,
        ITravelRequest travelRequestService) {
        _contextProvider = contextProvider;
        _travelRequestService = travelRequestService;
    }

    [HttpGet]
    public CommonResponse<ManagerInfo> GetManagerInfoByEmployeeId(Guid employeeId)
    {
        var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
        var result = _travelRequestService.GetManagerInfoByEmployeeId(sessionContext, employeeId);
        return CommonResponse.CreateSuccess(result);
    }

    [HttpPost]
    public void SetBusinessTripStatus([FromBody] BusinessTripRequest request)
    {
        var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
        _travelRequestService.SetBusinessTripStatus(sessionContext, request);
    }

    [HttpPost]
    public CommonResponse<TravelInfo> CalculateTravelInfo([FromBody] TravelInfoRequest request)
    {
        var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
        var result = _travelRequestService.CalculateTravelInfo(sessionContext, request);
        return CommonResponse.CreateSuccess(result);
    }

    [HttpPost]
    public CommonResponse<List<TicketInfo>> SearchTickets([FromBody] TicketInfoRequest request)
    {
        var sessionContext = _contextProvider.GetOrCreateCurrentSessionContext();
        var result = _travelRequestService.SearchTickets(sessionContext, request);
        if (result is null)
            return CommonResponse.CreateError<List<TicketInfo>>("Ошибка в запросе. Получен null.");
        return CommonResponse.CreateSuccess(result);
    }
}
