using System.Text;

namespace Task6.Model;

internal class TravelRequestGenerator {
	/// <summary>
	/// Сотрудники, которые будут добавляться в качестве: автора, согласующего, оформителя и командированного.
	/// </summary>
	public List<string> Employees { get; } = new List<string>();

	/// <summary>
	/// Список организаций-контрагентов для указания в заявке.
	/// </summary>
	public List<string> PartnerDepartments { get; } = new List<string>();

	/// <summary>
	/// Варианты причин для поездки.
	/// </summary>
	public List<string> Reasons { get; } = new List<string>();

	/// <summary>
	/// Варианты городов для поездки.
	/// </summary>
	public List<string> Сities { get; } = new List<string>();

	/// <summary>
	/// Имена возможных состояний.
	/// </summary>
	public List<string> StateNames { get; } = new List<string>();

	/// <summary>
	/// Начало номера заявки.
	/// </summary>
	public int BeginRequestNumber { get; set; } = 1;

	/// <summary>
	/// Минимальная дата создания.
	/// </summary>
	public DateTime MinCreatedDate { get; set; } = DateTime.Now;

	/// <summary>
	/// Максимальная дата создания.
	/// </summary>
	public DateTime MaxCreatedDate { get; set; } = DateTime.Now;

	/// <summary>
	/// Минимальное количество дней до начала командировки.
	/// </summary>
	public int MinDaysToStart { get; set; } = 2;

	/// <summary>
	/// Максимальное количество дней до начала командировки.
	/// </summary>
	public int MaxDaysToStart { get; set; } = 7;

	/// <summary>
	/// Минимальная длительность командировки (с учетом дня начала).
	/// </summary>
	public int MinTravelDays { get; set; } = 1;

	/// <summary>
	/// Максимальная длительность командировки (с учетом дня начала).
	/// </summary>
	public int MaxTravelDays { get; set; } = 14;

	/// <summary>
	/// Имена возможных прикрепляемых файлов.
	/// </summary>
	public List<string> AttachedFileNames { get; } = new List<string>();

	/// <summary>
	/// Проверяет установленные значения параметров на корректность.
	/// </summary>
	/// <exception cref="ArgumentException">Ошибка при проверке параметров.</exception>
	private void ValidateFields() {
		var sb = new StringBuilder();
		if (Employees.Count < 1) {
			sb.AppendLine("> Требуется задать список сотрудников");
		}
		if (PartnerDepartments.Count < 1) {
			sb.AppendLine("> Требуется задать список организаций-контрагентов");
		}
		if (Reasons.Count < 1) {
			sb.AppendLine("> Требуется задать список причин для поездки");
		}
		if (Сities.Count < 1) {
			sb.AppendLine("> Требуется задать список городов");
		}
		if (StateNames.Count < 1) {
			sb.AppendLine("> Требуется задать список имен состояний");
		}
		if (MaxCreatedDate < MinCreatedDate) {
			sb.AppendLine("> Максимальная дата создания должна быть больше или равна минимальной дате");
		}
		if (MaxDaysToStart < MinDaysToStart) {
			sb.AppendLine("> Максимальное количество дней до начала командировки должно быть больше или равно минимальному количеству");
		}
		if (MaxTravelDays < MinTravelDays) {
			sb.AppendLine("> Максимальное количество дней командировки должно быть больше или равно минимальному количеству");
		}
		if (MinTravelDays < 1) {
			sb.AppendLine("> Минимальное количество дней командировки должно быть положительным");
		}
		if (MaxTravelDays < 1) {
			sb.AppendLine("> Максимальное количество дней командировки должно быть положительным");
		}
		if (sb.Length > 0) {
			sb.Insert(0, "Ошибки в параметрах генератора: \n");
			throw new ArgumentException(sb.ToString());
		}
	}

	/// <summary>
	/// Случайно формирует дату создания между MaxCreatedDate и MinCreatedDate.
	/// </summary>
	/// <returns>Дата создания заявки.</returns>
	private DateTime GenerateRandomCreatedDate() {
		int days = (MaxCreatedDate - MinCreatedDate).Days;
		int randomDays = Random.Shared.Next(0, days + 1);
		return MinCreatedDate.AddDays(randomDays);
	}

	/// <summary>
	/// Относительно даты создания добавляет случайное количество дней до начала командировки.
	/// </summary>
	/// <returns>Даты командировки 'с'.</returns>
	private DateTime GenerateRandomFromDate(DateTime createdDate) {
		int randomDays = Random.Shared.Next(MinDaysToStart, MaxDaysToStart + 1);
		return createdDate.AddDays(randomDays);
	}

	/// <summary>
	/// Относительно даты начала командировки добавляет случайное количество дней до завершения командировки.
	/// </summary>
	/// <returns>Даты командировки 'по'.</returns>
	private DateTime GenerateRandomToDate(DateTime fromDate) {
		int randomDays = Random.Shared.Next(MinDaysToStart, MaxDaysToStart + 1);
		return fromDate.AddDays(randomDays);
	}

	/// <summary>
	/// Случайно выбирает строку из заданного списка.
	/// </summary>
	/// <param name="list">Список вариантов.</param>
	/// <returns>Случайно выбранная строка.</returns>
	private static string ChooseRandomString(List<string> list) {
		int i = Random.Shared.Next(0, list.Count);
		return list[i];
	}

	/// <summary>
	/// Случайно выбирает несколько строк (от 1 до max) из заданного списка.
	/// </summary>
	/// <param name="list">Список вариантов.</param>
	/// <param name="max">Максмально возможный выбор вариантов.</param>
	/// <returns>Случайно выбранные строки.</returns>
	private static List<string> ChooseRandomStrings(List<string> list, int max) {
		var newList = new List<string>();
		int n = Random.Shared.Next(1, max + 1);
		for (int j = 0; j < n; j++) {
			int i = Random.Shared.Next(0, list.Count);
			if (!newList.Contains(list[i])) {
				newList.Add(list[i]);
			}
		}
		return newList;
	}

	/// <summary>
	/// Создает список заявок на основе введенных параметров и генератора псевдослучайных чисел.
	/// </summary>
	/// <param name="count">Количество заявок.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentException">Ошибка при проверке параметров.</exception>
	public IList<RequiredTravelRequestData> Generate(int count) {
		if (count < 1) {
			throw new ArgumentException("Количество должно быть положительным значением.");
		}
		ValidateFields();
		var data = new List<RequiredTravelRequestData>(count);
		int n = BeginRequestNumber + count;
		for (int i = BeginRequestNumber; i < n; i++) {
			var createdDate = GenerateRandomCreatedDate();
			var fromDate = GenerateRandomFromDate(createdDate);
			var toDate = GenerateRandomToDate(fromDate);

			var author = ChooseRandomString(Employees);
			var partnerDepartments = ChooseRandomString(PartnerDepartments);
			var reason = ChooseRandomString(Reasons);
			var city = ChooseRandomString(Сities);
			var stateName = ChooseRandomString(StateNames);
			var ticketType = Random.Shared.Next(0, 2);
			var attachedFileNames = ChooseRandomStrings(AttachedFileNames, max: 3);
			
			data.Add(new RequiredTravelRequestData() {
				Name = $"Заявка №{i}",
				CreatedDate = createdDate,
				Author = author,
				FromDate = fromDate,
				ToDate = toDate,
				PartnerDepartment = partnerDepartments,
				Reason = reason,
				TicketType = (TravelRequestCard.TicketTypes)ticketType,
				City = city,
				StateName = stateName,
				AttachedFileName = attachedFileNames
			});
		}
		return data;
	}
}
