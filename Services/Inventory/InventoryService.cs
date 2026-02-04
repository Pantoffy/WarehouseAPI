using WarehouseAPI.DTOs.InventoryDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private IUOW UOW;

        public InventoryService(IUOW uow)
        {
            UOW = uow;
        }

        public async Task<InventoryResponse> AddInventoryAsync(CreateInventoryRequest inventory)
        {
            return await UOW.InventoryRepository.AddInventoryAsync(inventory);
        }

        public async Task<bool> DeleteInventoryByIdAsync(int id)
        {
            return await UOW.InventoryRepository.DeleteInventoryByIdAsync(id);
        }

        public async Task<List<InventoryResponse>> GetAllInventoryAsync()
        {
            return await UOW.InventoryRepository.GetAllInventoryAsync();
        }

        public async Task<InventoryResponse?> GetInventoryByIdAsync(int id)
        {
            return await UOW.InventoryRepository.GetInventoryByIdAsync(id);
        }

        public async Task<bool> UpdateInventoryByIdAsync(int id, UpdateInventoryRequest inventory)
        {
            return await UOW.InventoryRepository.UpdateInventoryByIdAsync(id, inventory);
        }
    }
}
