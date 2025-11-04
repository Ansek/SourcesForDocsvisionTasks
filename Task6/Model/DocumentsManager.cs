using DocsVision.BackOffice.CardLib.CardDefs;
using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectModel.Search;
using DocsVision.Platform.StorageServer;

namespace Task6.Model; 

internal delegate void AccessDeniedEventHandler(string message);

internal class DocumentsManager {
	/// <summary>
	/// Сессия.
	/// </summary>
	private readonly UserSession _session;

	/// <summary>
	/// Контекст.
	/// </summary>
	private readonly ObjectContext _context;

	/// <summary>
	/// Сервис конструктора состояний.
	/// </summary>
	private readonly IStateService _stateService;

	/// <summary>
	/// Вид карточки для заявки на командировку.
	/// </summary>
	private readonly KindsCardKind _travelRequestKind;

    /// <summary>
    /// Структура карточки заявки на командировку.
    /// </summary>
    private readonly TravelRequestCard _travelRequestCard;

    /// <summary>
    /// Информация о компании.
    /// </summary>
    public CompanyInfo CompanyInfo { get; }

	/// <summary>
	/// Менеджер для управлении документами.
	/// </summary>
	/// <param name="session">Сессия.</param>
	/// <param name="context">Контекст.</param>
	/// <param name="companyName">Название организации.</param>
	public DocumentsManager(UserSession session, ObjectContext context, string? companyName = null) {
		_session = session;
		_context = context;
		CompanyInfo = new CompanyInfo(context, companyName);
		_stateService = _context.GetService<IStateService>();
		_travelRequestKind = _context.FindObject<KindsCardKind>(
			new QueryObject(
				KindsCardKind.NameProperty.Name,
				TravelRequestCard.KindName));
		_travelRequestCard = new TravelRequestCard(session);
    }

	/// <summary>
	/// Возращает список доступных состояний документа.
	/// </summary>
	public IList<StatesState> States => _stateService.GetStates(_travelRequestKind);

	/// <summary>
	/// Событие на исключение из-за прав доступа.
	/// </summary>
	public event AccessDeniedEventHandler? OnAccessDenied;

	/// <summary>
	/// Осуществляет поиск пути из ребер до нужного состояния.
	/// </summary>
	/// <param name="currentState">Текущее состояние.</param>
	/// <param name="stateName">Имя искомого состояния.</param>
	/// <returns>Путь до состояния.</returns>
	private IEnumerable<StatesStateMachineBranch> FindPathToState(
		StatesState? currentState, string stateName) {
		if (currentState == null) {
			return Enumerable.Empty<StatesStateMachineBranch>();
		}
		var path = new Stack<StatesStateMachineBranch>();
		var visited = new HashSet<StatesState> { currentState };

		do {
			var branches = _stateService.FindLineBranchesByStartState(currentState);
			currentState = null;
			foreach (var branch in branches) {
				if (!visited.Contains(branch.EndState)) {
					currentState = branch.EndState;
					path.Push(branch);
					visited.Add(currentState);
					break;
				}
			}
			if (currentState?.DefaultName == stateName ||
				currentState?.LocalizedName == stateName) {
				break;
			}
			if (currentState == null && path.Count > 0) {
				currentState = path.Pop().StartState;
			}
		} while (path.Count > 0);

		return path.Reverse();
	}

	/// <summary>
	/// Изменяет состояние документа.
	/// </summary>
	/// <param name="doc">Документ.</param>
	/// <param name="stateName">Название устанавливаемого состояние.</param>
	public void ChangeState(Document doc, string stateName) {
		var current = doc.SystemInfo.State;
		if (current.DefaultName == stateName ||
			current.LocalizedName == stateName) {
			return;
		}

		var path = FindPathToState(current, stateName);

		foreach (var branch in path) {
			_stateService.ChangeState(doc, branch);
			_context.AcceptChanges();
		}
	}

	/// <summary>
	/// Загружает документы заявок на сервер.
	/// </summary>
	/// <param name="documents">Данные документов для заполнения.</param>
	public void UploadDocuments(IList<RequiredTravelRequestData> documents) {
		foreach (var doc in documents) {
			var author = CompanyInfo.GetEmployeeByDisplayName(doc.Author);
			var city = CompanyInfo.GetCityByName(doc.City);
			var partner = CompanyInfo.GetPartnerDepartmentName(doc.PartnerDepartment);
			var secretary = CompanyInfo.GetSecretary();
			var manager = CompanyInfo.GetManagerByEmployee(author);
			var dayCount = (doc.ToDate - doc.FromDate).Days + 1;

			var docService = _context.GetService<IDocumentService>();
			var travelRequest = docService.CreateDocument(null, _travelRequestKind);
			travelRequest.MainInfo.Name = doc.Name;
			travelRequest.MainInfo.Author = author;
			travelRequest.MainInfo.Registrar = author;
			
			var dataSection = new BaseCardSectionRow();
			dataSection[TravelRequestCard.FromDate] = doc.FromDate;
			dataSection[TravelRequestCard.ToDate] = doc.ToDate;
			dataSection[TravelRequestCard.City] = city.GetObjectId();
			dataSection[TravelRequestCard.DayCount] = dayCount;
			dataSection[TravelRequestCard.Amount] = 1000 * dayCount;				
			dataSection[TravelRequestCard.PartnerDepartment] = partner.GetObjectId();
			dataSection[TravelRequestCard.Reason] = doc.Reason;
			dataSection[TravelRequestCard.TicketType] = doc.TicketType;
			dataSection[TravelRequestCard.Traveler] = author.GetObjectId();
			dataSection[TravelRequestCard.Manager] = manager.GetObjectId();
			dataSection[TravelRequestCard.ManagerPhone] = manager.Phone;
			
			var applicantSection = new BaseCardSectionRow();
			applicantSection[TravelRequestCard.Applicant] = secretary.GetObjectId();

			var approverSection = new BaseCardSectionRow();
			approverSection[TravelRequestCard.Approver] = manager.GetObjectId();

			var data = travelRequest.GetSection(_travelRequestCard.TravelRequestSectionId);
			var applicants = travelRequest.GetSection(_travelRequestCard.ApplicantsSectionId);
			var approvers = travelRequest.GetSection(_travelRequestCard.ApproversSectionId);

			data.Add(dataSection);
			applicants.Add(applicantSection);
			approvers.Add(approverSection);
			_context.AcceptChanges();

			foreach(var fileName in doc.AttachedFileName) {
				if (File.Exists(fileName)) {
					docService.AddMainFile(travelRequest, fileName);
				}
			}
			ChangeState(travelRequest, doc.StateName);
		}
	}

	/// <summary>
	/// Удаляет все документы на сервере.
	/// </summary>
	public void DeleteAllDocuments() {
		var docCards = _session.CardManager.GetCards(CardDocument.ID);
		foreach (var card in docCards) {
			try {
				_session.CardManager.DeleteCard(card.Id);
			} catch (StorageServerException ex) {
				if (ex.ErrorCode != (int)ErrorCode.AccessDenied) {
					OnAccessDenied?.Invoke($"Не удалось удалить документ {card.Id} из-за прав доступа");
				} else {
					throw;
				}
			}
		}
		_context.AcceptChanges();
	}
}
