using WarehouseAPI.DTOs.WarehouseDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Warehouse
{
    public class WarehouseService : IWarehouseService
    {
        private IUOW UOW;

        public WarehouseService(IUOW uow)
        {
            UOW = uow;
        }

        public async Task<WarehouseResponse> AddWarehouseAsync(CreateWarehouseRequest warehouse)
        {
            return await UOW.WarehouseRepository.AddWarehouseAsync(warehouse);
        }

        public async Task<bool> DeleteWarehouseByIdAsync(int id)
        {
            return await UOW.WarehouseRepository.DeleteWarehouseByIdAsync(id);
        }

        public async Task<List<WarehouseResponse>> GetAllWarehouseAsync()
        {
            return await UOW.WarehouseRepository.GetAllWarehouseAsync();
        }

        public async Task<WarehouseResponse?> GetWarehouseByIdAsync(int id)
        {
            return await UOW.WarehouseRepository.GetWarehouseByIdAsync(id);
        }

        public async Task<bool> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouse)
        {
            return await UOW.WarehouseRepository.UpdateWarehouseByIdAsync(id, warehouse);
        }
    }
}
