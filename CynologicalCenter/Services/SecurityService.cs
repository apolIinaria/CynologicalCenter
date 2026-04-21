using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data;
using CynologicalCenter.Services.Interfaces;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace CynologicalCenter.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly DbConnectionFactory _factory;
        public SecurityService(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<(bool Success, string Role)> LoginAsync(
            string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                return (false, string.Empty);

            try
            {
                using var conn = _factory.CreateConnection();
                await conn.OpenAsync();

                using var cmd = new MySqlCommand(
                    "SELECT password_hash, salt FROM appusers WHERE username = @u", conn);
                cmd.Parameters.AddWithValue("@u", username);

                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return (false, string.Empty);

                string storedHash = reader.GetString(reader.GetOrdinal("password_hash"));
                string salt = reader.GetString(reader.GetOrdinal("salt"));
                reader.Close();

                string inputHash = ComputeHash(password, salt);

                if (!inputHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase))
                    return (false, string.Empty);

                string role = ResolveRole(username);
                return (true, role);
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        private string ComputeHash(string password, string salt)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(password + salt));
            return BitConverter.ToString(bytes)
                .Replace("-", string.Empty)
                .ToLower();
        }

        // Роль визначається по імені облікового запису
        private string ResolveRole(string username) => username switch
        {
            "polina_admin" => "admin",
            "marusych_op" => "operator",
            "stajer_guest" => "guest",
            _ => "guest"
        };
    }
}
