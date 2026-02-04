using WarehouseAPI.DTOs.InventoryDTOs;

namespace WarehouseAPI.Services.Inventory
{
    public interface IInventoryService
    {
        Task<List<InventoryResponse>> GetAllInventoryAsync();
        Task<InventoryResponse?> GetInventoryByIdAsync(int id);
        Task<InventoryResponse> AddInventoryAsync(CreateInventoryRequest inventory);
        Task<bool> UpdateInventoryByIdAsync(int id, UpdateInventoryRequest inventory);
        Task<bool> DeleteInventoryByIdAsync(int id);
    }
}
