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
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DbConnectionFactory _factory;
        public OwnerRepository(DbConnectionFactory factory) => _factory = factory;
        private Owner Map(MySqlDataReader r) => new Owner
        {
            OwnerId = r.GetInt32("owner_id"),
            FullName = r.GetString("full_name"),
            Phone = r.IsDBNull(r.GetOrdinal("phone")) ? null : r.GetString("phone"),
            Email = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
            IsActive = r.GetBoolean("is_active")
        };

        public async Task<List<Owner>> GetAllAsync()
        {
            var list = new List<Owner>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT owner_id, full_name, phone, email, is_active FROM owners", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<Owner>> GetActiveAsync()
        {
            var list = new List<Owner>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT owner_id, full_name, phone, email, is_active FROM owners WHERE is_active = 1", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Owner?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT owner_id, full_name, phone, email, is_active FROM owners WHERE owner_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return Map(r);
        }

        public async Task AddAsync(Owner owner)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "INSERT INTO owners (full_name, phone, email) VALUES (@n, @p, @e)", conn);
            cmd.Parameters.AddWithValue("@n", owner.FullName);
            cmd.Parameters.AddWithValue("@p", (object?)owner.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)owner.Email ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Owner owner)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE owners SET full_name=@n, phone=@p, email=@e WHERE owner_id=@id", conn);
            cmd.Parameters.AddWithValue("@n", owner.FullName);
            cmd.Parameters.AddWithValue("@p", (object?)owner.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@e", (object?)owner.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", owner.OwnerId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<ActiveClientViewModel>> GetActiveClientsViewAsync()
        {
            var list = new List<ActiveClientViewModel>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM v_active_clients", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new ActiveClientViewModel
                {
                    OwnerId = r.GetInt32("owner_id"),
                    FullName = r.GetString("full_name"),
                    Phone = r.IsDBNull(r.GetOrdinal("phone")) ? null : r.GetString("phone"),
                    Email = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
                    DogsCount = r.GetInt32("dogs_count"),
                    LastVisit = r.IsDBNull(r.GetOrdinal("last_visit")) ? null : r.GetDateTime("last_visit")
                });
            return list;
        }
    }
}