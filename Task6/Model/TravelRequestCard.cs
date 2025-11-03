namespace Task6.Model;

internal class TravelRequestCard {

	/// <summary>
	/// Идентификатор секции "Командировка".
	/// </summary>
	public static readonly Guid ID = new("CE5F1DD4-EF59-43AE-AE39-2E186605CABA");

	/// <summary>
	/// Название вида документа.
	/// </summary>
	public static readonly string KindName = "Заявка на командировку";

	/// <summary>
	/// Даты командировки 'с'.
	/// </summary>
	public const string FromDate = "fromTravelDate";

	/// <summary>
	/// Даты командировки 'по'.
	/// </summary>
	public const string ToDate = "toTravelDate";

	/// <summary>
	/// Кол-во дней в командировке.
	/// </summary>
	public const string DayCount = "dayCount";

	/// <summary>
	/// Сумма командировочных.
	/// </summary>
	public const string Amount = "amountTravel";

	/// <summary>
	/// Организация.
	/// </summary>
	public const string PartnerDepartment = "partnersDepartment";

	/// <summary>
	/// Основание для поездки.
	/// </summary>
	public const string Reason = "reasonTravel";

	/// <summary>
	/// Константы для секции "ОформляющиеКомандировку".
	/// </summary>
	public static class Applicants {
		/// <summary>
		/// Идентификатор секции "ОформляющиеКомандировку".
		/// </summary>
		public static readonly Guid ID = new("590B4AF7-42D3-4588-8C70-18C213C66CD6");

		/// <summary>
		/// Оформитель.
		/// </summary>
		public const string Applicant = "applicant";
	}

	/// <summary>
	/// Константы для секции "СогласущиеКомандировку".
	/// </summary>
	public static class Approvers {
		/// <summary>
		/// Идентификатор секции "СогласущиеКомандировку".
		/// </summary>
		public static readonly Guid ID = new("81ABBA60-9EEB-4870-B172-D88E9B30DC0E");

		/// <summary>
		/// Согласующий.
		/// </summary>
		public const string Approver = "approver";
	}

	/// <summary>
	/// Тип билета.
	/// </summary>
	public const string TicketType = "tickets";

	/// <summary>
	/// Командируемый.
	/// </summary>
	public const string Traveler = "traveler";

	/// <summary>
	/// Город.
	/// </summary>
	public const string City = "city";

	/// <summary>
	/// Руководитель.
	/// </summary>
	public const string Manager = "manager";

	/// <summary>
	/// Телефон.
	/// </summary>
	public const string ManagerPhone = "phoneNumberManager";

	/// <summary>
	/// Перечисление типов билетов.
	/// </summary>
	public enum TicketTypes {
		/// <summary>
		/// Авиа.
		/// </summary>
		Avia = 0,
		/// <summary>
		/// Поезд.
		/// </summary>
		Train = 1
	}

	/// <summary>
	/// Название справочника городов.
	/// </summary>
	public const string CityDirectoryName = "Города";
}
