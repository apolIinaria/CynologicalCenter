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
    public class BreedRepository : IBreedRepository
    {
        private readonly DbConnectionFactory _factory;
        public BreedRepository(DbConnectionFactory factory) => _factory = factory;
        public async Task<List<Breed>> GetAllAsync()
        {
            var list = new List<Breed>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT breed_id, breed_name FROM breeds ORDER BY breed_name", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new Breed { BreedId = r.GetInt32(0), BreedName = r.GetString(1) });
            return list;
        }

        public async Task<Breed?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT breed_id, breed_name FROM breeds WHERE breed_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return new Breed { BreedId = r.GetInt32(0), BreedName = r.GetString(1) };
        }

        public async Task AddAsync(Breed breed)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "INSERT INTO breeds (breed_name) VALUES (@name)", conn);
            cmd.Parameters.AddWithValue("@name", breed.BreedName);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Breed breed)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE breeds SET breed_name = @name WHERE breed_id = @id", conn);
            cmd.Parameters.AddWithValue("@name", breed.BreedName);
            cmd.Parameters.AddWithValue("@id", breed.BreedId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "DELETE FROM breeds WHERE breed_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}