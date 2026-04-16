using WarehouseAPI.DTOs.StockDTOs;

namespace WarehouseAPI.Services.Stock
{
    public interface IStockService
    {
        Task<StockResponse?> GetByIdAsync(int id);
        Task<List<StockResponse>> GetAllAsync();
        Task<List<StockResponse>> GetByWarehouseIdAsync(int warehouseId);
        Task<StockResponse> CreateAsync(CreateStockRequest request);
        Task<StockResponse> UpdateAsync(int id, UpdateStockRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
