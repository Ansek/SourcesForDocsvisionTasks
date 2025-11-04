using DocsVision.Platform.ObjectManager;

namespace Task6.Model;

internal class TravelRequestCard {

    public TravelRequestCard(UserSession session)
    {
        var docCardType = session.CardManager.CardTypes.Where(x => x.Name == "Документ").First();
        if (docCardType != null) { 
            TravelRequestSectionId = docCardType.Sections.Where(x => x.Name == "Командировка").First().Id;
            ApplicantsSectionId = docCardType.Sections.Where(x => x.Name == "ОформляющиеКомандировку").First().Id;
            ApproversSectionId = docCardType.Sections.Where(x => x.Name == "СогласующиеКомандировку").First().Id;
        }
    }

    /// <summary>
    /// Идентификатор секции "Командировка".
    /// </summary>
    public readonly Guid TravelRequestSectionId;

    /// <summary>
    /// Идентификатор секции "ОформляющиеКомандировку".
    /// </summary>
    public readonly Guid ApplicantsSectionId;

    /// <summary>
    /// Идентификатор секции "СогласующиеКомандировку".
    /// </summary>
    public readonly Guid ApproversSectionId;

    /// <summary>
    /// Название вида документа.
    /// </summary>
    public const string KindName = "Заявка на командировку";

    /// <summary>
    /// Название справочника городов.
    /// </summary>
    public const string CityDirectoryName = "Города";

    /// <summary>
    /// Перечисление типов билетов.
    /// </summary>
    public enum TicketTypes
    {
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
    /// Оформитель.
    /// </summary>
    public const string Applicant = "applicant";

    /// <summary>
    /// Согласующий.
    /// </summary>
    public const string Approver = "approver";

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
}
