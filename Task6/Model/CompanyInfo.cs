using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.ObjectModel;

namespace Task6.Model;

internal class CompanyInfo {
	/// <summary>
	/// Название группы для секретарей.
	/// </summary>
	private const string SecretaryGroupName = "Секретарь";

	/// <summary>
	/// Контекст.
	/// </summary>
	private readonly ObjectContext _context;

	/// <summary>
	/// Сервис справочника сотрудников.
	/// </summary>
	private readonly IStaffService _staffService;

	/// <summary>
	/// Сервис справочника контрагентов.
	/// </summary>
	private readonly IPartnersService _partnersService;

	/// <summary>
	/// Сервис конструктора справочника.
	/// </summary>
	private readonly IBaseUniversalService _baseUniversalService;

	/// <summary>
	/// Данные о структуре организации.
	/// </summary>
	private readonly StaffUnit _company;

	/// <summary>
	/// Группа секретаря.
	/// </summary>
	private readonly StaffGroup _secretaryGroup;

	/// <summary>
	/// Информация о компании.
	/// </summary>
	/// <param name="context">Контекст.</param>
	/// <param name="companyName">Название организации.</param>
	public CompanyInfo(ObjectContext context, string? companyName) {
		_context = context;
		_staffService = _context.GetService<IStaffService>();
		_partnersService = _context.GetService<IPartnersService>();
		_baseUniversalService = _context.GetService<IBaseUniversalService>();
		_company = _staffService.FindCompanyByNameOnServer(null, companyName);
		_secretaryGroup = _staffService.FindGroupByName(null, SecretaryGroupName);
	}

	/// <summary>
	/// Возвращает список сотрудников компании.
	/// </summary>
	public IEnumerable<StaffEmployee> Employees =>
		_company.Units.SelectMany(unit => unit.Employees.Where(x => x.AccountName != "ENGINEER\\admin"));
		
	/// <summary>
	/// Возвращает список организаций-контрагентов.
	/// </summary>
	public IEnumerable<PartnersCompany> PartnerDepartments =>
		_partnersService.GetUnits(null, true);

	/// <summary>
	/// Возвращает варианты городов для поездки.
	/// </summary>
	public IEnumerable<BaseUniversalItem> Сities =>
		_baseUniversalService.FindItemTypeWithSameName(TravelRequestCard.CityDirectoryName, null).Items;

	/// <summary>
	/// Возращает идентификатор первого активного секретаря.
	/// </summary>
	/// <returns>Объект сотрудника.</returns>
	public StaffEmployee GetSecretary() =>
		_secretaryGroup.Employees.Where(x => x.Status == StaffEmployeeStatus.Active).First();

	/// <summary>
	/// Возвращает первого сотрудника, который содержит displayName.
	/// </summary>
	/// <param name="searchName">Строка поиска.</param>
	/// <returns>Объект сотрудника.</returns>
	public StaffEmployee GetEmployeeByDisplayName(string searchName) =>
		Employees.Where(x => x.DisplayName.Contains(searchName)).First();

	/// <summary>
	/// Возвращает руководителя сотрудника.
	/// </summary>
	/// <param name="employee">Сотрудник.</param>
	/// <returns>Объект руководителя сотрудника.</returns>
	public StaffEmployee GetManagerByEmployee(StaffEmployee employee) =>
		_staffService.GetEmployeeManager(employee);

	/// <summary>
	/// Возвращает первого сотрудника c именем searchCity.
	/// </summary>
	/// <param name="searchCity">Строка поиска.</param>
	/// <returns>Объект справочника.</returns>
	public BaseUniversalItem GetCityByName(string searchCity) =>
		Сities.Where(x => x.Name == searchCity).First();

	/// <summary>
	/// Возвращает первого контрагента c именем searchPartner.
	/// </summary>
	/// <param name="searchCity">Строка поиска.</param>
	/// <returns>Объект контрагента.</returns>
	public PartnersCompany GetPartnerDepartmentName(string searchPartner) =>
		PartnerDepartments.Where(x => x.Name == searchPartner).First();
}
