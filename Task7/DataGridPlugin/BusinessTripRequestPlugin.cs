using DocsVision.BackOffice.ObjectModel;
using DocsVision.Layout.WebClient.Models;
using DocsVision.Layout.WebClient.Models.TableData;
using DocsVision.Layout.WebClient.Services;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.ExtensionMethods;
using Task7.Const;

namespace Task7.DataGridPlugin
{
    internal class BusinessTripRequestPlugin : IDataGridControlPlugin
    {
        public string Name => "TravelRequestPlugin";

        private const string RowNumber = "rowNumber";
        private const string FromDate = "fromDate";
        private const string City = "city";
        private const string Reason = "reason";
        private const string State = "state";

        private static TableModel CreateTable()
        {
            var table = new TableModel();
            table.Columns.Add(new ColumnModel()
            {
                Id = RowNumber,
                Name = "№",
                Type = DocsVision.WebClient.Models.Grid.ColumnType.Integer
            });
            table.Columns.Add(new ColumnModel()
            {
                Id = FromDate,
                Name = "Дата выезда",
                Type = DocsVision.WebClient.Models.Grid.ColumnType.String,
            });
            table.Columns.Add(new ColumnModel()
            {
                Id = City,
                Name = "Город",
                Type = DocsVision.WebClient.Models.Grid.ColumnType.String
            });
            table.Columns.Add(new ColumnModel()
            {
                Id = Reason,
                Name = "Основание для поездки",
                Type = DocsVision.WebClient.Models.Grid.ColumnType.String
            });
            table.Columns.Add(new ColumnModel()
            {
                Id = State,
                Name = "Статус заявки",
                Type = DocsVision.WebClient.Models.Grid.ColumnType.String
            });
            table.Id = Guid.NewGuid().ToString();
            return table;
        }

        private static void FillTable(TableModel table, InfoRowCollection reader)
        {
            int i = 1;
            foreach(var row in reader)
            {
                var fromDate = ((DateTime)row["FromDate"]).ToString("dd.MM.yyyy");
                table.Rows.Add(new RowModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Cells = new List<CellModel>()
                    {
                        new() { ColumnId = RowNumber, Value = i++ },
                        new() { ColumnId = FromDate, Value = fromDate },
                        new() { ColumnId = City, Value = row["City"] },
                        new() { ColumnId = Reason, Value = row["Reason"] },
                        new() { ColumnId = State, Value = row["State"] }
                    }
                });
            }
        }

        public TableModel GetTableData(SessionContext sessionContext, List<ParamModel> parameters)
        {
            var table = CreateTable();
            var parameter = parameters.FirstOrDefault(x => x.Key == "CurrentCardId");
            if (Guid.TryParse(parameter?.Value, out Guid cardId)) {
                var doc = sessionContext.ObjectContext.GetObject<Document>(cardId);
                var card = new TravelRequestCard(sessionContext.Session);
                var section = doc.GetSection(card.TravelRequestSectionId).
                    GetFirstOrDefault<BaseCardSectionRow>();
                if (section != null && section[TravelRequestCard.Traveler] is Guid travelerId)
                {
                    var getBusinessTripHistory = sessionContext.Session.ExtensionManager
                        .GetExtensionMethod("BusinessTripRequestExtension", "GetBusinessTripHistory");
                    getBusinessTripHistory.Parameters.AddNew("employeeId",
                        ParameterValueType.Guid, travelerId);

                    var reader = getBusinessTripHistory.ExecuteReader();
                    if (reader != null)
                        FillTable(table, reader);
                }
            }
            return table;
        }
    }
}
