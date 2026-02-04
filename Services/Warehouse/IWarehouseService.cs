using WarehouseAPI.DTOs.WarehouseDTOs;

namespace WarehouseAPI.Services.Warehouse
{
    public interface IWarehouseService
    {
        Task<List<WarehouseResponse>> GetAllWarehouseAsync();
        Task<WarehouseResponse?> GetWarehouseByIdAsync(int id);
        Task<WarehouseResponse> AddWarehouseAsync(CreateWarehouseRequest warehouse);
        Task<bool> UpdateWarehouseByIdAsync(int id, UpdateWarehouseRequest warehouse);
        Task<bool> DeleteWarehouseByIdAsync(int id);
    }
}
