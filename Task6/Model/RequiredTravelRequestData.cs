namespace Task6.Model;

internal class RequiredTravelRequestData {
	/// <summary>
	/// Название заявки.
	/// </summary>
	public string Name { get; set; } = "";

	/// <summary>
	/// Дата создания.
	/// </summary>
	public DateTime CreatedDate { get; set; }

	/// <summary>
	/// Название заявки.
	/// </summary>
	public string Author { get; set; } = "";

	/// <summary>
	/// Даты командировки 'с'.
	/// </summary>
	public DateTime FromDate { get; set; }

	/// <summary>
	/// Даты командировки 'по'.
	/// </summary>
	public DateTime ToDate { get; set; }

	/// <summary>
	/// Организация.
	/// </summary>
	public string PartnerDepartment { get; set; } = "";

	/// <summary>
	/// Основание для поездки.
	/// </summary>
	public string Reason { get; set; } = "";

	/// <summary>
	/// Тип билета.
	/// </summary>
	public TravelRequestCard.TicketTypes TicketType { get; set; } = TravelRequestCard.TicketTypes.Avia;

	/// <summary>
	/// Город.
	/// </summary>
	public string City { get; set; } = "";

	/// <summary>
	/// Имя прикрепляемого файл.
	/// </summary>
	public List<string> AttachedFileName;

	/// <summary>
	/// Данные необходимые для заполнения заявки на командировку.
	/// </summary>
	public RequiredTravelRequestData() {
		AttachedFileName = new List<string>();
	}

	/// <summary>
	/// Название состояния документа.
	/// </summary>
	public string StateName { get; set; } = "";

	public override string ToString() {
		return $"{Name}, создано {CreatedDate: dd.MM.yy} ({StateName}), " +
			$"командировка от {FromDate: dd.MM.yy} до {ToDate: dd.MM.yy}, " +
			$"автор: {Author}, куда: {PartnerDepartment} ({City}, {TicketType}), файлов: {AttachedFileName.Count}";
	}
}
