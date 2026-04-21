using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;
using MySql.Data.MySqlClient;
using CynologicalCenter.Helpers;

namespace CynologicalCenter.Data.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DbConnectionFactory _factory;
        public SessionRepository(DbConnectionFactory factory) => _factory = factory;
        private Session Map(MySqlDataReader r) => new Session
        {
            SessionId = r.GetInt32("session_id"),
            DogId = r.IsDBNull(r.GetOrdinal("dog_id")) ? null : r.GetInt32("dog_id"),
            DogNickname = r.HasColumn("nickname") && !r.IsDBNull(r.GetOrdinal("nickname")) ? r.GetString("nickname") : null,
            TrainerId = r.IsDBNull(r.GetOrdinal("trainer_id")) ? null : r.GetInt32("trainer_id"),
            TrainerName = r.HasColumn("trainer_name") && !r.IsDBNull(r.GetOrdinal("trainer_name")) ? r.GetString("trainer_name") : null,
            CourseId = r.IsDBNull(r.GetOrdinal("course_id")) ? null : r.GetInt32("course_id"),
            CourseName = r.HasColumn("course_name") && !r.IsDBNull(r.GetOrdinal("course_name")) ? r.GetString("course_name") : null,
            SessionDatetime = r.GetDateTime("session_datetime"),
            Status = r.GetString("status"),
            Grade = r.IsDBNull(r.GetOrdinal("grade")) ? null : r.GetDecimal("grade"),
            Comment = r.IsDBNull(r.GetOrdinal("comment")) ? null : r.GetString("comment"),
            DurationMinutes = r.GetInt32("duration_minutes")
        };

        public async Task<List<Session>> GetAllAsync()
        {
            var list = new List<Session>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT s.*, d.nickname, t.full_name AS trainer_name, c.course_name
                FROM sessions s
                LEFT JOIN dogs d ON s.dog_id = d.dog_id
                LEFT JOIN trainers t ON s.trainer_id = t.trainer_id
                LEFT JOIN courses c ON s.course_id = c.course_id
                ORDER BY s.session_datetime DESC", conn);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Session?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT s.*, d.nickname, t.full_name AS trainer_name, c.course_name
                FROM sessions s
                LEFT JOIN dogs d ON s.dog_id = d.dog_id
                LEFT JOIN trainers t ON s.trainer_id = t.trainer_id
                LEFT JOIN courses c ON s.course_id = c.course_id
                WHERE s.session_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return Map(r);
        }

        public async Task<List<Session>> GetByDogIdAsync(int dogId)
        {
            var list = new List<Session>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT s.*, d.nickname, t.full_name AS trainer_name, c.course_name
                FROM sessions s
                LEFT JOIN dogs d ON s.dog_id = d.dog_id
                LEFT JOIN trainers t ON s.trainer_id = t.trainer_id
                LEFT JOIN courses c ON s.course_id = c.course_id
                WHERE s.dog_id = @id ORDER BY s.session_datetime DESC", conn);
            cmd.Parameters.AddWithValue("@id", dogId);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<Session>> GetByTrainerIdAsync(int trainerId)
        {
            var list = new List<Session>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT s.*, d.nickname, t.full_name AS trainer_name, c.course_name
                FROM sessions s
                LEFT JOIN dogs d ON s.dog_id = d.dog_id
                LEFT JOIN trainers t ON s.trainer_id = t.trainer_id
                LEFT JOIN courses c ON s.course_id = c.course_id
                WHERE s.trainer_id = @id ORDER BY s.session_datetime", conn);
            cmd.Parameters.AddWithValue("@id", trainerId);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<Session>> GetByStatusAsync(string status)
        {
            var list = new List<Session>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT s.*, d.nickname, t.full_name AS trainer_name, c.course_name
                FROM sessions s
                LEFT JOIN dogs d ON s.dog_id = d.dog_id
                LEFT JOIN trainers t ON s.trainer_id = t.trainer_id
                LEFT JOIN courses c ON s.course_id = c.course_id
                WHERE s.status = @st ORDER BY s.session_datetime", conn);
            cmd.Parameters.AddWithValue("@st", status);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task UpdateStatusAsync(int sessionId, string status)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE sessions SET status = @s WHERE session_id = @id", conn);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", sessionId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PublicScheduleViewModel>> GetPublicScheduleViewAsync()
        {
            var list = new List<PublicScheduleViewModel>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM v_public_schedule", conn);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new PublicScheduleViewModel
                {
                    SessionTime = r.GetString("час_заняття"),
                    Trainer = r.GetString("тренер"),
                    Course = r.GetString("курс"),
                    Price = r.IsDBNull(r.GetOrdinal("ціна_грн")) ? null : r.GetDecimal("ціна_грн"),
                    Dog = r.GetString("собака"),
                    Breed = r.GetString("порода"),
                    Status = r.GetString("статус"),
                    DurationMinutes = r.GetInt32("тривалість_хв")
                });
            return list;
        }
    }
}
