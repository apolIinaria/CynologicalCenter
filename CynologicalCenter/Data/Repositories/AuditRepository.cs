using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Models;
using MySql.Data.MySqlClient;

namespace CynologicalCenter.Data.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly DbConnectionFactory _factory;
        public AuditRepository(DbConnectionFactory factory) => _factory = factory;
        private AuditLog Map(MySqlDataReader r) => new AuditLog
        {
            LogId = r.GetInt32("log_id"),
            TableName = r.IsDBNull(r.GetOrdinal("table_name")) ? null : r.GetString("table_name"),
            RecordId = r.IsDBNull(r.GetOrdinal("record_id")) ? null : r.GetInt32("record_id"),
            Action = r.IsDBNull(r.GetOrdinal("action")) ? null : r.GetString("action"),
            OldValue = r.IsDBNull(r.GetOrdinal("old_value")) ? null : r.GetString("old_value"),
            NewValue = r.IsDBNull(r.GetOrdinal("new_value")) ? null : r.GetString("new_value"),
            ChangedAt = r.GetDateTime("changed_at"),
            ChangedBy = r.IsDBNull(r.GetOrdinal("changed_by")) ? null : r.GetString("changed_by"),
            Description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description"),
            OperationSource = r.IsDBNull(r.GetOrdinal("operation_source")) ? null : r.GetString("operation_source")
        };

        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            var list = new List<AuditLog>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT * FROM auditlog
                WHERE DATE(changed_at) BETWEEN @f AND @t
                ORDER BY changed_at DESC", conn);
            cmd.Parameters.AddWithValue("@f", from.Date);
            cmd.Parameters.AddWithValue("@t", to.Date);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<AuditLog>> GetByTableAsync(string tableName)
        {
            var list = new List<AuditLog>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT * FROM auditlog WHERE table_name = @t ORDER BY changed_at DESC", conn);
            cmd.Parameters.AddWithValue("@t", tableName);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<AuditLog>> GetErrorsAsync()
        {
            var list = new List<AuditLog>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT * FROM auditlog WHERE action = 'ERROR' ORDER BY changed_at DESC", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }
    }
}