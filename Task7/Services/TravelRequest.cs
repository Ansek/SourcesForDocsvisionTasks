using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.WebClient;
using Task7.Const;
using Task7.Models;

namespace Task7.Services
{
    public class TravelRequest : ITravelRequest
    {
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
