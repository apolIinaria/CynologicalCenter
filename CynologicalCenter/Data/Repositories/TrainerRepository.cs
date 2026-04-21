using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;
using MySql.Data.MySqlClient;

namespace CynologicalCenter.Data.Repositories
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly DbConnectionFactory _factory;
        public TrainerRepository(DbConnectionFactory factory) => _factory = factory;
        private Trainer Map(MySqlDataReader r) => new Trainer
        {
            TrainerId = r.GetInt32("trainer_id"),
            FullName = r.GetString("full_name"),
            Phone = r.IsDBNull(r.GetOrdinal("phone")) ? null : r.GetString("phone"),
            Email = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
            ExperienceYears = r.IsDBNull(r.GetOrdinal("experience_years")) ? null : r.GetInt32("experience_years"),
            HireDate = r.IsDBNull(r.GetOrdinal("hire_date")) ? null : r.GetDateTime("hire_date"),
            PhotoPath = r.IsDBNull(r.GetOrdinal("photo_path")) ? null : r.GetString("photo_path")
        };

        public async Task<List<Trainer>> GetAllAsync()
        {
            var list = new List<Trainer>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT * FROM trainers ORDER BY full_name", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Trainer?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT * FROM trainers WHERE trainer_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return Map(r);
        }

        public async Task AddAsync(Trainer t)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                INSERT INTO trainers (full_name, phone, email, experience_years, hire_date)
                VALUES (@n, @p, @e, @exp, @h)", conn);
            cmd.Parameters.AddWithValue("@n", t.FullName);
            cmd.Parameters.AddWithValue("@p", (object?)t.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)t.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@exp", (object?)t.ExperienceYears ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@h", (object?)t.HireDate ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Trainer t)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                UPDATE trainers SET full_name=@n, phone=@p, email=@e,
                experience_years=@exp, hire_date=@h WHERE trainer_id=@id", conn);
            cmd.Parameters.AddWithValue("@n", t.FullName);
            cmd.Parameters.AddWithValue("@p", (object?)t.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)t.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@exp", (object?)t.ExperienceYears ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@h", (object?)t.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", t.TrainerId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "DELETE FROM trainers WHERE trainer_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePhotoAsync(int trainerId, string relativePath)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE trainers SET photo_path = @p WHERE trainer_id = @id", conn);
            cmd.Parameters.AddWithValue("@p", relativePath);
            cmd.Parameters.AddWithValue("@id", trainerId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<int>> GetCourseIdsAsync(int trainerId)
        {
            var list = new List<int>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT course_id FROM trainer_courses WHERE trainer_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", trainerId);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(r.GetInt32(0));
            return list;
        }

        public async Task SetCoursesAsync(int trainerId, List<int> courseIds)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var del = new MySqlCommand(
                "DELETE FROM trainer_courses WHERE trainer_id = @id", conn);
            del.Parameters.AddWithValue("@id", trainerId);
            await del.ExecuteNonQueryAsync();

            foreach (var cid in courseIds)
            {
                using var ins = new MySqlCommand(
                    "INSERT INTO trainer_courses (trainer_id, course_id) VALUES (@t, @c)", conn);
                ins.Parameters.AddWithValue("@t", trainerId);
                ins.Parameters.AddWithValue("@c", cid);
                await ins.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<TrainerKpiViewModel>> GetKpiViewAsync()
        {
            var list = new List<TrainerKpiViewModel>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM v_trainer_kpi", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new TrainerKpiViewModel
                {
                    TrainerId = r.GetInt32("trainer_id"),
                    TrainerName = r.GetString("trainer_name"),
                    TotalSessions = r.GetInt32("total_sessions"),
                    AvgGrade = r.IsDBNull(r.GetOrdinal("avg_grade")) ? null : r.GetDecimal("avg_grade"),
                    Rating = r.GetInt64("rating")
                });
            return list;
        }
    }
}