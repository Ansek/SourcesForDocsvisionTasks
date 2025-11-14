using DocsVision.Platform.ObjectManager;

namespace Task7.Const;

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
    /// Название группы для секретарей.
    /// </summary>
    public const string SecretaryGroupName = "Секретарь";

    /// <summary>
    /// Cуточные.
    /// </summary>
    public const string DailyAllowance = "DailyAllowance";

    /// <summary>
    /// IATA-код города или аэропорта.
    /// </summary>
    public const string IATA = "IATA";

    /// <summary>
    /// Оформитель.
    /// </summary>
    public const string Applicant = "applicant";

    /// <summary>
    /// Согласующий.
    /// </summary>
    public const string Approver = "approver";

	/// <summary>
	/// Командируемый.
	/// </summary>
	public const string Traveler = "traveler";

    /// <summary>
    /// Руководитель.
    /// </summary>
    public const string Manager = "manager";

    /// <summary>
    /// Телефон.
    /// </summary>
    public const string ManagerPhone = "phoneNumberManager";
}
