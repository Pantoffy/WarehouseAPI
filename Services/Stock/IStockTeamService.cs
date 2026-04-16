using WarehouseAPI.DTOs.StockDTOs;

namespace WarehouseAPI.Services.Stock
{
    public interface IStockTeamService
    {
        Task<StockTeamResponse?> GetByIdAsync(int id);
        Task<List<StockTeamResponse>> GetAllAsync();
        Task<List<StockTeamResponse>> GetByStockCheckIdAsync(int stockCheckId);
        Task<StockTeamResponse> CreateAsync(CreateStockTeamRequest request);
        Task<StockTeamResponse> UpdateAsync(int id, CreateStockTeamRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByStockCheckIdAsync(int stockCheckId);
    }
}
