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
    public class CourseRepository : ICourseRepository
    {
        private readonly DbConnectionFactory _factory;
        public CourseRepository(DbConnectionFactory factory) => _factory = factory;
        private Course Map(MySqlDataReader r) => new Course
        {
            CourseId = r.GetInt32("course_id"),
            CourseName = r.GetString("course_name"),
            Description = r.IsDBNull(r.GetOrdinal("description")) ? null : r.GetString("description"),
            Price = r.IsDBNull(r.GetOrdinal("price")) ? null : r.GetDecimal("price"),
            MinAgeMonths = r.IsDBNull(r.GetOrdinal("min_age_months")) ? null : r.GetInt32("min_age_months")
        };

        public async Task<List<Course>> GetAllAsync()
        {
            var list = new List<Course>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM courses ORDER BY course_name", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT * FROM courses WHERE course_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return Map(r);
        }

        public async Task AddAsync(Course c)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                INSERT INTO courses (course_name, description, price, min_age_months)
                VALUES (@n, @d, @p, @m)", conn);
            cmd.Parameters.AddWithValue("@n", c.CourseName);
            cmd.Parameters.AddWithValue("@d", (object?)c.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p", (object?)c.Price ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@m", (object?)c.MinAgeMonths ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Course c)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                UPDATE courses SET course_name=@n, description=@d,
                price=@p, min_age_months=@m WHERE course_id=@id", conn);
            cmd.Parameters.AddWithValue("@n", c.CourseName);
            cmd.Parameters.AddWithValue("@d", (object?)c.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p", (object?)c.Price ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@m", (object?)c.MinAgeMonths ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", c.CourseId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "DELETE FROM courses WHERE course_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}