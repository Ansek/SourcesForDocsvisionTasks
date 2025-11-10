using System.Text;
using System.Text.RegularExpressions;

namespace Task9.Model
{
    internal record Observer(
        string FIO,
        string Position,
        List<string>? Cities);

    internal class ObserverDepartment
    {
        public string Name { get; } 
        public List<ObserverDepartment>? Subdepartments { get; set; } = null;
        public List<Observer>? Observers { get; set; } = null;
        public ObserverDepartment(string name) => Name = name.Trim();
    }

    internal class CSVParser
    {
        public static ObserverDepartment ParseObserverDepartment(string filePath)
        {
            var root = new ObserverDepartment("root") {
                Subdepartments = new List<ObserverDepartment>()
            };
            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                var data = Regex.Replace(line, @"\s+", " ").Split(',');
                var department = GetOrCreateDepartment(root, data[0].Trim().Split('\\'));
                department.Observers?.Add(new(
                    data[1].Trim(),
                    data[2].Trim(),
                    ParseObservedCities(data[3].Trim())));
            }
            return root;
        }

        private static ObserverDepartment GetOrCreateDepartment(ObserverDepartment department, IEnumerable<string> keys)
        {
            var key = keys.FirstOrDefault();
            if (key == null) {
                department.Observers ??= new();
                return department;
            }
            ObserverDepartment? select = null;
            department.Subdepartments ??= new();
            foreach (var subdepartment in department.Subdepartments) {
                if (subdepartment.Name == key) {
                    select = subdepartment;
                    break;
                }
            }
            if (select == null) {
                select = new ObserverDepartment(key);
                department.Subdepartments.Add(select);
            }
            return GetOrCreateDepartment(select, keys.Skip(1));
        }

        private static List<string>? ParseObservedCities(string data)
        {
            var cities = new List<string>();
            foreach(var city in data.Split()) {
                var pair = city.Split(':');
                if (pair[1].ToLower() == "да")
                    cities.Add(pair[0]);
            }
            if (cities.Count == 0) {
                return null; 
            }
            return cities;
        }
    }
}
