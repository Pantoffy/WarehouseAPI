using WarehouseAPI.DTOs.StockDTOs;

namespace WarehouseAPI.Services.Stock
{
    public interface IStockDetailService
    {
        Task<StockDetailResponse?> GetByIdAsync(int id);
        Task<List<StockDetailResponse>> GetByStockCheckIdAsync(int stockCheckId);
        Task<StockDetailResponse> CreateAsync(CreateStockDetailRequest request);
        Task<StockDetailResponse> UpdateAsync(int id, CreateStockDetailRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByStockCheckIdAsync(int stockCheckId);
    }
}
