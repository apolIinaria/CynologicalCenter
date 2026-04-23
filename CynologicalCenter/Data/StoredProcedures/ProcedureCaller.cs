using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace CynologicalCenter.Data.StoredProcedures
{
    public class ProcedureCaller
    {
        private readonly DbConnectionFactory _factory;
        public ProcedureCaller(DbConnectionFactory factory) => _factory = factory;

        // sp_enroll_dog
        public async Task<string> EnrollDogAsync(
            int dogId, int trainerId, int courseId, DateTime dt)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_enroll_dog", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_dog_id", dogId);
            cmd.Parameters.AddWithValue("p_trainer_id", trainerId);
            cmd.Parameters.AddWithValue("p_course_id", courseId);
            cmd.Parameters.AddWithValue("p_datetime", dt);
            var outMsg = new MySqlParameter("p_message", MySqlDbType.VarChar, 255)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outMsg);

            await cmd.ExecuteNonQueryAsync();
            return outMsg.Value?.ToString() ?? string.Empty;
        }

        // sp_complete_session
        public async Task<string> CompleteSessionAsync(
            int sessionId, decimal grade, string comment)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_complete_session", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_session_id", sessionId);
            cmd.Parameters.AddWithValue("p_grade", grade);
            cmd.Parameters.AddWithValue("p_comment", comment);
            var outMsg = new MySqlParameter("p_message", MySqlDbType.VarChar, 255)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outMsg);

            await cmd.ExecuteNonQueryAsync();
            return outMsg.Value?.ToString() ?? string.Empty;
        }

        // sp_deactivate_owner
        public async Task<string> DeactivateOwnerAsync(int ownerId)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_deactivate_owner", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_owner_id", ownerId);
            var outMsg = new MySqlParameter("p_message", MySqlDbType.VarChar, 255)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outMsg);

            await cmd.ExecuteNonQueryAsync();
            return outMsg.Value?.ToString() ?? string.Empty;
        }

        // sp_get_trainer_schedule
        public async Task<DataSet> GetTrainerScheduleAsync(int trainerId, DateTime date)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_get_trainer_schedule", conn)
            { CommandType = System.Data.CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_trainer_id", trainerId);
            cmd.Parameters.AddWithValue("p_date", date.Date);

            var ds = new DataSet();
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);
            return ds;
        }

        // sp_generate_dog_report
        public async Task<DataSet> GenerateDogReportAsync(int dogId)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_generate_dog_report", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_dog_id", dogId);

            var ds = new DataSet();
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);
            return ds;
        }

        // sp_analyze_logs
        public async Task<DataSet> AnalyzeLogsAsync(DateTime from, DateTime to)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_analyze_logs", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_date_from", from.Date);
            cmd.Parameters.AddWithValue("p_date_to", to.Date);

            var ds = new DataSet();
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);
            return ds;
        }

        // sp_check_security_incidents
        public async Task<(DataSet Data, int TotalIncidents)> CheckSecurityAsync(int hoursBack)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("sp_check_security_incidents", conn)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("p_hours_back", hoursBack);
            var outTotal = new MySqlParameter("p_total_incidents", MySqlDbType.Int32)
            { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outTotal);

            var ds = new DataSet();
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);

            int total = outTotal.Value is DBNull ? 0 : Convert.ToInt32(outTotal.Value);
            return (ds, total);
        }
    }
}