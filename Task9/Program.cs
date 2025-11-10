using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Task9.Model;

namespace Task9;

internal class Program
{
    private const string CSVFileName = "Данные для загрузки в систему.csv";
    private const string CSVFileURL = @"https://s1239sas.storage.yandex.net/rdisk/4c0f3ac1b06f2726623ee606f68de2646e024dbe4c499bccb889f102f5c89400/690fae47/BpJfPUGB8Fwu_vNDFMPeiWCKNw0nuVF2aWQAPmN4u9ISmqy3bKua5KTVTepdFmZQrO_JDtJCD6BE50b8tdPyUg==?uid=0&filename=%D0%94%D0%B0%D0%BD%D0%BD%D1%8B%D0%B5%20%D0%B4%D0%BB%D1%8F%20%D0%B7%D0%B0%D0%B3%D1%80%D1%83%D0%B7%D0%BA%D0%B8%20%D0%B2%20%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D1%83.csv&disposition=attachment&hash=bdDc0RbCZd0dEIEadtlMRkB8leFdQHjTH1M4pxu0a2sLGav5wD8TzTIEfV4QDaL7q%2FJ6bpmRyOJonT3VoXnDag%3D%3D%3A%2F%D0%A8%D0%BA%D0%BE%D0%BB%D0%B0%20%D0%98%D0%BD%D0%B6%D0%B5%D0%BD%D0%B5%D1%80%D0%B0%20-%20%D0%BE%D0%BA%D1%82%D1%8F%D0%B1%D1%80%D1%8C%202025.%20%D0%97%D0%B0%D0%BF%D0%B8%D1%81%D0%B8%20%D0%B8%20%D0%B7%D0%B0%D0%B4%D0%B0%D0%BD%D0%B8%D1%8F%20%D0%BE%D1%81%D0%BD%D0%BE%D0%B2%D0%BD%D0%BE%D0%B3%D0%BE%20%D0%B1%D0%BB%D0%BE%D0%BA%D0%B0%2F11_05_2025.%20%D0%9B%D0%B5%D0%BA%D1%86%D0%B8%D1%8F%20%D0%B8%20%D0%B7%D0%B0%D0%B4%D0%B0%D0%BD%D0%B8%D0%B5%209%2F%D0%94%D0%B0%D0%BD%D0%BD%D1%8B%D0%B5%20%D0%B4%D0%BB%D1%8F%20%D0%B7%D0%B0%D0%B3%D1%80%D1%83%D0%B7%D0%BA%D0%B8%20%D0%B2%20%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D1%83.csv&limit=0&content_type=text%2Fcsv&owner_uid=210599063&fsize=10912242&hid=86e83875b41c87a514a551401aee532a&media_type=data&tknv=v3&ts=6431b8502dfc0&s=7c2e001b2da7460539c110213441ab511effc6b3a02aee1683c5a9e76ed72931&pb=U2FsdGVkX1-k3y-lAOphrqSRmp3kW4ywHqSwohOQ0I3_bSS9_qOoaZ75lByDlIIapAgoPkwKfTyZJVmLogrCl2yV-oxWhBT_RL6o2eBj8d8";

    private static void Main()
    {
        EnsureCSVFileExists();
        var root = CSVParser.ParseObserverDepartment(CSVFileName);
        var json = ConvertToJSON(root.Subdepartments);
        var info = GetCountInfo(root);
        Console.WriteLine($"В файле найдено:");
        Console.WriteLine($"- всего подразделений: {info.DepartmentCount};");
        Console.WriteLine($"- всего наблюдателей: {info.ObserverCount}.");
        Console.WriteLine($"Выполнение запроса...");
        using var dbSession = new DBSession();
        var task = dbSession.AddObserverDepartmentsAsync(json);
        task.Wait();
        Console.WriteLine($"Выполнение завершено.");
    }

    private static void EnsureCSVFileExists()
    {
        if (!File.Exists(CSVFileName))
        {
            Console.WriteLine("Скачивание файла...");
            using var httpClient = new HttpClient();
            var response = httpClient.GetAsync(CSVFileURL).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            using var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
            using var fileStream = File.Create(CSVFileName);
            stream.CopyTo(fileStream);
        }
    }

    private static string ConvertToJSON(object? obj)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(obj, options);
    }

    private record CountInfo(int DepartmentCount, int ObserverCount);

    private static CountInfo GetCountInfo(ObserverDepartment department)
    {
        int departmentCount = 0, observerCount = 0;
        var stack = new Stack<ObserverDepartment>();
        stack.Push(department);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.Subdepartments != null) {
                departmentCount += current.Subdepartments.Count;
                foreach (var subdepartment in current.Subdepartments) {
                    stack.Push(subdepartment);
                }
            }
            if (current.Observers != null) {
                observerCount += current.Observers.Count;
            }
        }
        return new CountInfo(departmentCount, observerCount);
    }
}

