using System.Text.Json;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectModel;
using Task6.Model;

namespace Task6;

internal class Program {

	public const string CompanyName = "Микоян";
	public const string JSONFile = "DataForTask5.json";

	public static void Main() {
		while (true) {
			Console.WriteLine("1. Добавить заявки из json-файла (для 5 задания)");
			Console.WriteLine("2. Добавить сгенерированные заявки (для 6 задания)");
			Console.WriteLine("3. Удалить все документы");
			Console.WriteLine("0. Выход");
			Console.Write("> ");
			if (!Int32.TryParse(Console.ReadLine(), out int cmd) ||
				cmd < 0 || cmd > 3) {
				Console.WriteLine("Команда не распознана.");
			} else if (cmd == 0) {
				break;
			} else {
				DVSessionAction action = cmd switch {
					1 => UploadDocumentsFromJson,
					2 => UploadGeneratedDocuments,
					_ => DeleteAllDocuments
				};
				try {
					var dvSession = new DVSession();
					dvSession.Run(action);
				} catch (ArgumentException ex) {
					Console.WriteLine(ex.Message);
				}
			}
		}
		Console.WriteLine("Press any key to continue...");
		Console.ReadKey();
	}

	private static void UploadDocumentsFromJson(UserSession session, ObjectContext context) {
		var json = File.ReadAllText(JSONFile);
		var docs = JsonSerializer.Deserialize<List<RequiredTravelRequestData>>(json);
		if (docs != null) {
			var docManager = new DocumentsManager(session, context, CompanyName);
			docManager.UploadDocuments(docs);
		}
	}

	private static void UploadGeneratedDocuments(UserSession session, ObjectContext context) {
		Console.Write("Количество заявок для генерации = ");
		if (!Int32.TryParse(Console.ReadLine(), out int count) || count < 1) {
			Console.WriteLine("Количество должно быть больше 0.");
		} else {
			var docManager = new DocumentsManager(session, context, CompanyName);
			var generator = new TravelRequestGenerator();
			ConfigureGenerator(generator, docManager);
			var docs = generator.Generate(count);
			docManager.UploadDocuments(docs);
		}
	}

	private static void DeleteAllDocuments(UserSession session, ObjectContext context) {
		var docManager = new DocumentsManager(session, context);
		docManager.OnAccessDenied += Console.WriteLine;
		docManager.DeleteAllDocuments();
	}

	private static void ConfigureGenerator(TravelRequestGenerator generator, DocumentsManager docManager) {
		generator.Employees.AddRange(docManager.CompanyInfo.Employees.Select(x => x.DisplayName));
		generator.PartnerDepartments.AddRange(docManager.CompanyInfo.PartnerDepartments.Select(x => x.Name));
		generator.Reasons.Add("Повышение квалификации.");
		generator.Reasons.Add("Настройка оборудования.");
		generator.Reasons.Add("Заключение договоров.");
		generator.Сities.AddRange(docManager.CompanyInfo.Сities.Select(x => x.Name));
		generator.StateNames.AddRange(docManager.States.Select(x => x.DefaultName));
		generator.BeginRequestNumber = 1;
		generator.MinCreatedDate = new DateTime(2025, 09, 01);
		generator.MaxCreatedDate = new DateTime(2025, 12, 01);
		generator.MinDaysToStart = 2;
		generator.MaxDaysToStart = 7;
		generator.MinTravelDays = 1;
		generator.MaxTravelDays = 14;
		generator.AttachedFileNames.Add("");
		for (int i = 1; i <= 5; i++) {
			var fileName = $"file{i}.txt";
			File.WriteAllText(fileName, $"Содержимое файла {i}.");
			generator.AttachedFileNames.Add(fileName);
		}
	}
}
