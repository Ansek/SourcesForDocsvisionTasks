using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.WebClient;
using Task7.Const;
using Task7.Models;

namespace Task7.Services
{
    public class TravelRequest : ITravelRequest
    {
        private const string OriginIATA = "LED";
        private const string APIToken = "b165d8c4be5500d4da61df5067fd34ad";

        private static StaffEmployee GetManagerByEmployee(IStaffService staffService, StaffEmployee employee)
        {
            var manager = staffService.GetEmployeeManager(employee);
            if (manager == employee && manager.Unit.ParentUnit != null)
            {
                manager = manager.Unit.ParentUnit.Manager;
            }
            return manager;
        }

        public ManagerInfo GetManagerInfoByEmployeeId(SessionContext sessionContext, Guid employeeId)
        {
            var context = sessionContext.ObjectContext;
            var employee = context.GetObject<StaffEmployee>(employeeId);
            var staffService = context.GetService<IStaffService>()
                ?? throw new InvalidOperationException("Сервис IStaffService не найден.");
            var manager = GetManagerByEmployee(staffService, employee);
            return new ManagerInfo { Id = manager.GetObjectId(), Phone = manager.Phone };
        }

        public TravelInfo CalculateTravelInfo(SessionContext sessionContext, TravelInfoRequest request)
        {
            var context = sessionContext.ObjectContext;
            var city = context.GetObject<BaseUniversalItem>(request.CityId);
            var dailyAllowance = (decimal)city.ItemCard.MainInfo[TravelRequestCard.DailyAllowance];
            var dayCount = (request.ToDate.Date - request.FromDate.Date).Days + 1;
            var amount = dailyAllowance * dayCount;
            return new TravelInfo { Amount = amount, DayCount = dayCount };
        }

        public void SetBusinessTripStatus(SessionContext sessionContext, BusinessTripRequest request)
        {
            var context = sessionContext.ObjectContext;
            var employee = context.GetObject<StaffEmployee>(request.EmployeeId);
            employee.InactiveStatus = StaffEmployeeInactiveStatus.BusinessTrip;
            employee.StartDate = request.FromDate.AddHours(3); // GMT+3
            employee.EndDate = request.ToDate.AddHours(3);
            context.SaveObject(employee);
        }

        private record ApiResponse(
            [property: JsonPropertyName("data")] List<TicketInfo> Data
        );

        public List<TicketInfo>? SearchTickets(SessionContext sessionContext, TicketInfoRequest request)
        {
            var context = sessionContext.ObjectContext;
            var destination = context.GetObject<BaseUniversalItem>(request.DestinationId);
            var destinationIATA = (string)destination.ItemCard.MainInfo[TravelRequestCard.IATA];

            var api = new StringBuilder();
            api.Append("https://api.travelpayouts.com/aviasales/v3/prices_for_dates");
            api.Append("?origin=");
            api.Append(OriginIATA);
            api.Append("&destination=");
            api.Append(destinationIATA);
            api.Append("&departure_at=");
            api.Append(request.DepartureAt.AddHours(3).ToString("yyyy-MM-dd"));
            api.Append("&return_at=");
            api.Append(request.ReturnAt.AddHours(3).ToString("yyyy-MM-dd"));
            api.Append("&unique=false&sorting=price&direct=true&currency=rub&limit=10&page=1&one_way=true");
            api.Append("&token=");
            api.Append(APIToken);

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(api.ToString()).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            TicketInfo.NextId = 0;
            return JsonSerializer.Deserialize<ApiResponse>(json)?.Data;
        }

        public void InitTravelRequestKind(SessionContext sessionContext, Guid cardId)
        {
            var card = new TravelRequestCard(sessionContext.Session);
            var context = sessionContext.ObjectContext;
            var staffService = context.GetService<IStaffService>();

            var author = sessionContext.UserInfo.Employee;
            var secretaryGroup = staffService.FindGroupByName(null, TravelRequestCard.SecretaryGroupName);
            var secretary = secretaryGroup.Employees.Where(x => x.Status == StaffEmployeeStatus.Active).First();           
            var manager = GetManagerByEmployee(staffService, author);

            var travelRequest = sessionContext.ObjectContext.GetObject<Document>(cardId);
            travelRequest.MainInfo.Author = author;
            travelRequest.MainInfo.Registrar = author;

            var dataSection = new BaseCardSectionRow();
            dataSection[TravelRequestCard.Traveler] = author.GetObjectId();
            dataSection[TravelRequestCard.Manager] = manager.GetObjectId();
            dataSection[TravelRequestCard.ManagerPhone] = manager.Phone;

            var applicantSection = new BaseCardSectionRow();
            applicantSection[TravelRequestCard.Applicant] = secretary.GetObjectId();

            var approverSection = new BaseCardSectionRow();
            approverSection[TravelRequestCard.Approver] = manager.GetObjectId();

            var data = travelRequest.GetSection(card.TravelRequestSectionId);
            var applicants = travelRequest.GetSection(card.ApplicantsSectionId);
            var approvers = travelRequest.GetSection(card.ApproversSectionId);

            data.Add(dataSection);
            applicants.Add(applicantSection);
            approvers.Add(approverSection);

            context.SaveObject(travelRequest);
        }
    }
}
