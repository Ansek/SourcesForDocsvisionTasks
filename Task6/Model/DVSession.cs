using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;

namespace Task6.Model;

/// <summary>
/// Дейстивие, выполняемое при открытой сессии.
/// </summary>
/// <param name="session">Сессия.</param>
/// <param name="context">Контекст.</param>
internal delegate void DVSessionAction(UserSession session, ObjectContext context);

internal class DVSession {
	/// <summary>
	/// Менеджер сессии.
	/// </summary>
	private readonly SessionManager _sessionManager;

	/// <summary>
	/// Объест для хранения данных сессии. 
	/// </summary>
	/// <param name="serverURL">Адрес сервера.</param>
	/// <param name="username">Логин пользователя.</param>
	/// <param name="password">Пароль.</param>
	public DVSession(string? serverURL = null, string? username = null, string? password = null) {
		serverURL ??= System.Configuration.ConfigurationManager.AppSettings["DVUrl"];
		username ??= System.Configuration.ConfigurationManager.AppSettings["Username"];
		password ??= System.Configuration.ConfigurationManager.AppSettings["Password"];
		_sessionManager = SessionManager.CreateInstance();
		_sessionManager.Connect(serverURL, String.Empty, username, password);
	}

	/// <summary>
	/// Выполнение действий внутри сессии.
	/// </summary>
	/// <param name="action">Выполняемое действие.</param>
	public void Run(DVSessionAction action) {
		UserSession? session = null;
		try {
			session = _sessionManager.CreateSession();
			var context = ContextFactory.CreateContext(session);
			action(session, context);
		} finally {
			session?.Close();
		}
	}
}
