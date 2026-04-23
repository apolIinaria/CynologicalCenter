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
    public class DogRepository : IDogRepository
    {
        private readonly DbConnectionFactory _factory;
        public DogRepository(DbConnectionFactory factory) => _factory = factory;
        private Dog Map(MySqlDataReader r) => new Dog
        {
            DogId = r.GetInt32("dog_id"),
            Nickname = r.GetString("nickname"),
            BreedId = r.IsDBNull(r.GetOrdinal("breed_id")) ? null : r.GetInt32("breed_id"),
            BreedName = r.HasColumn("breed_name") && !r.IsDBNull(r.GetOrdinal("breed_name")) ? r.GetString("breed_name") : null,
            BirthDate = r.IsDBNull(r.GetOrdinal("birth_date")) ? null : r.GetDateTime("birth_date"),
            OwnerId = r.IsDBNull(r.GetOrdinal("owner_id")) ? null : r.GetInt32("owner_id"),
            OwnerName = r.HasColumn("owner_name") && !r.IsDBNull(r.GetOrdinal("owner_name")) ? r.GetString("owner_name") : null,
            LastVaccination = r.IsDBNull(r.GetOrdinal("last_vaccination")) ? null : r.GetDateTime("last_vaccination"),
            PhotoPath = r.IsDBNull(r.GetOrdinal("photo_path")) ? null : r.GetString("photo_path")
        };

        public async Task<List<Dog>> GetAllAsync()
        {
            var list = new List<Dog>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT d.*, b.breed_name, o.full_name AS owner_name
                FROM dogs d
                LEFT JOIN breeds b ON d.breed_id = b.breed_id
                LEFT JOIN owners o ON d.owner_id = o.owner_id
                ORDER BY d.nickname", conn);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<Dog>> GetByOwnerIdAsync(int ownerId)
        {
            var list = new List<Dog>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT d.*, b.breed_name, o.full_name AS owner_name
                FROM dogs d
                LEFT JOIN breeds b ON d.breed_id = b.breed_id
                LEFT JOIN owners o ON d.owner_id = o.owner_id
                WHERE d.owner_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", ownerId);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Dog?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT d.*, b.breed_name, o.full_name AS owner_name
                FROM dogs d
                LEFT JOIN breeds b ON d.breed_id = b.breed_id
                LEFT JOIN owners o ON d.owner_id = o.owner_id
                WHERE d.dog_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return Map(r);
        }

        public async Task AddAsync(Dog d)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                INSERT INTO dogs (nickname, breed_id, birth_date, owner_id, last_vaccination)
                VALUES (@n, @b, @bd, @o, @v)", conn);
            cmd.Parameters.AddWithValue("@n", d.Nickname);
            cmd.Parameters.AddWithValue("@b", (object?)d.BreedId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@bd", (object?)d.BirthDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@o", (object?)d.OwnerId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@v", (object?)d.LastVaccination ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Dog d)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                UPDATE dogs SET nickname=@n, breed_id=@b, birth_date=@bd,
                owner_id=@o, last_vaccination=@v WHERE dog_id=@id", conn);
            cmd.Parameters.AddWithValue("@n", d.Nickname);
            cmd.Parameters.AddWithValue("@b", (object?)d.BreedId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@bd", (object?)d.BirthDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@o", (object?)d.OwnerId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@v", (object?)d.LastVaccination ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", d.DogId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePhotoAsync(int dogId, string relativePath)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE dogs SET photo_path = @p WHERE dog_id = @id", conn);
            cmd.Parameters.AddWithValue("@p", relativePath);
            cmd.Parameters.AddWithValue("@id", dogId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<DogProfileViewModel>> GetProfilesViewAsync()
        {
            var list = new List<DogProfileViewModel>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM v_dog_training_profile", conn);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(MapProfile(r));
            return list;
        }

        public async Task<DogProfileViewModel?> GetProfileViewAsync(int dogId)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "SELECT * FROM v_dog_training_profile WHERE dog_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", dogId);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            return MapProfile(r);
        }

        public async Task<List<ExpiredVaccinationViewModel>> GetExpiredVaccinationViewAsync()
        {
            var list = new List<ExpiredVaccinationViewModel>();
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT * FROM v_expired_vaccination", conn);
            using var r = (MySqlDataReader)await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new ExpiredVaccinationViewModel
                {
                    DogId = r.GetInt32("dog_id"),
                    Nickname = r.GetString("nickname"),
                    BreedName = r.GetString("breed_name"),
                    OwnerName = r.GetString("owner_name"),
                    Phone = r.IsDBNull(r.GetOrdinal("phone")) ? null : r.GetString("phone"),
                    LastVaccination = r.IsDBNull(r.GetOrdinal("last_vaccination")) ? null : r.GetDateTime("last_vaccination"),
                    DaysSinceVaccination = r.GetInt32("days_since_vaccination")
                });
            return list;
        }

        public async Task DeleteAsync(int dogId)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "DELETE FROM dogs WHERE dog_id = @id", conn);
            cmd.Parameters.AddWithValue("@id", dogId);
            await cmd.ExecuteNonQueryAsync();
        }

        private DogProfileViewModel MapProfile(MySqlDataReader r) => new DogProfileViewModel
        {
            DogId = r.GetInt32("dog_id"),
            Nickname = r.GetString("кличка"),
            Breed = r.GetString("порода"),
            AgeMonths = r.IsDBNull(r.GetOrdinal("вік_місяців")) ? 0 : r.GetInt32("вік_місяців"),
            Owner = r.GetString("власник"),
            Phone = r.IsDBNull(r.GetOrdinal("телефон")) ? null : r.GetString("телефон"),
            TotalSessions = r.GetInt32("всього_занять"),
            CompletedSessions = r.GetInt32("виконано_занять"),
            AvgGrade = r.IsDBNull(r.GetOrdinal("середня_оцінка")) ? null : r.GetDecimal("середня_оцінка"),
            LastSession = r.IsDBNull(r.GetOrdinal("останнє_заняття")) ? null : r.GetDateTime("останнє_заняття"),
            LastVaccination = r.IsDBNull(r.GetOrdinal("остання_вакцинація")) ? null : r.GetDateTime("остання_вакцинація"),
            DaysSinceVaccination = r.IsDBNull(r.GetOrdinal("днів_після_вакцинації")) ? 0 : r.GetInt32("днів_після_вакцинації")
        };
    }
}
