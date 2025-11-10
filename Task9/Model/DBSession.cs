using Npgsql;
using NpgsqlTypes;
using System.Configuration;

namespace Task9.Model
{
    internal class DBSession : IDisposable
    {
        private const string AddObserverDepartmentsFuncName = "dd_add_observer_departments";
        private const string CreateTableFileName = "Server\\dd_create.sql";
        private readonly NpgsqlConnection _connection;

        public DBSession()
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            if (!ProcedureExists(AddObserverDepartmentsFuncName)) {
                CreateTable(CreateTableFileName);
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private void CreateTable(string fileName)
        {
            var sql = File.ReadAllText(fileName);
            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }

        private bool ProcedureExists(string procedureName)
        {
            const string sql = @"
                select exists (
	                select 1
	                from pg_proc
	                where proname = @procname
                );";            
            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("procname", procedureName);
            return (bool)(cmd.ExecuteScalar() ?? false);
        }

        public async Task AddObserverDepartmentsAsync(string json)
        {
            const string sql = $"call {AddObserverDepartmentsFuncName}(@departments);";
            using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("departments", NpgsqlDbType.Json, json);
            cmd.CommandTimeout = 600;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
