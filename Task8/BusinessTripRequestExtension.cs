using System.Data;
using DocsVision.Platform.Data;
using DocsVision.Platform.StorageServer;
using DocsVision.Platform.StorageServer.Extensibility;

namespace Task8;

public sealed class BusinessTripRequestExtension : StorageServerExtension
{
    public BusinessTripRequestExtension() { }

    [ExtensionMethod]
    public CursorInfo GetBusinessTripHistory(Guid travelerId)
    {
        using DatabaseCommand Command = DbRequest.DataLayer.Connection.
            CreateCommand("dd_business_trip_history", CommandType.StoredProcedure);
        Command.AddParameter("TravelerId", DbType.Guid, ParameterDirection.Input, 0, travelerId);
        return ExecuteCursorCommand(Command);
    }
}