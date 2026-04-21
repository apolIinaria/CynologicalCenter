using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CynologicalCenter.Data.Repositories.Interfaces;
using CynologicalCenter.Data.StoredProcedures;
using CynologicalCenter.Models;
using CynologicalCenter.Models.ViewModels;
using CynologicalCenter.Services.Interfaces;

namespace CynologicalCenter.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _owners;
        private readonly ProcedureCaller _procedures;
        public OwnerService(IOwnerRepository owners, ProcedureCaller procedures)
        {
            _owners = owners;
            _procedures = procedures;
        }

        public Task<List<Owner>> GetAllAsync()
            => _owners.GetAllAsync();

        public Task<List<Owner>> GetActiveAsync()
            => _owners.GetActiveAsync();

        public Task<Owner?> GetByIdAsync(int id)
            => _owners.GetByIdAsync(id);

        public Task<List<ActiveClientViewModel>> GetActiveClientsViewAsync()
            => _owners.GetActiveClientsViewAsync();

        public async Task<(bool Success, string Message)> AddAsync(Owner owner)
        {
            if (string.IsNullOrWhiteSpace(owner.FullName))
                return (false, "ПІБ є обов'язковим полем");

            if (!string.IsNullOrWhiteSpace(owner.Email) && !owner.Email.Contains('@'))
                return (false, "Невірний формат email");
            try
            {
                await _owners.AddAsync(owner);
                return (true, "Клієнта успішно додано");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Duplicate"))
                    return (false, "Клієнт з таким email вже існує");
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Owner owner)
        {
            if (string.IsNullOrWhiteSpace(owner.FullName))
                return (false, "ПІБ є обов'язковим полем");

            if (!string.IsNullOrWhiteSpace(owner.Email) && !owner.Email.Contains('@'))
                return (false, "Невірний формат email");

            try
            {
                await _owners.UpdateAsync(owner);
                return (true, "Дані клієнта оновлено");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Duplicate"))
                    return (false, "Клієнт з таким email вже існує");
                return (false, $"Помилка: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeactivateAsync(int ownerId)
        {
            // Вся логіка - в sp_deactivate_owner
            // (перевіряє існування, активність, наявність запланованих занять)
            try
            {
                string msg = await _procedures.DeactivateOwnerAsync(ownerId);
                bool success = msg.StartsWith("Успішно");
                return (success, msg);
            }
            catch (Exception ex)
            {
                return (false, $"Помилка: {ex.Message}");
            }
        }
    }
}
