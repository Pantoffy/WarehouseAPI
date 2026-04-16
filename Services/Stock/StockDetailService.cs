using WarehouseAPI.DTOs.StockDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Stock
{
    public class StockDetailService : IStockDetailService
    {
        private readonly StockCheckDetailRepository _repository;

        public StockDetailService(StockCheckDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<StockDetailResponse?> GetByIdAsync(int id)
        {
            var detail = await _repository.GetByIdAsync(id);
            return detail == null ? null : MapToResponse(detail);
        }

        public async Task<List<StockDetailResponse>> GetByStockCheckIdAsync(int stockCheckId)
        {
            var details = await _repository.GetByStockCheckIdAsync(stockCheckId);
            return details.Select(MapToResponse).ToList();
        }

        public async Task<StockDetailResponse> CreateAsync(CreateStockDetailRequest request)
        {
            var detail = new StockCheckDetail
            {
                StockCheckId = request.StockCheckId,
                MaterialId = request.MaterialId,
                SystemQuantity = request.SystemQuantity,
                ActualQuantity = request.ActualQuantity,
                Difference = request.ActualQuantity - request.SystemQuantity,
                HandlingProposal = request.HandlingProposal,
                RecordedCheck = true,
                Status = "Chưa xử lý"
            };

            var created = await _repository.CreateAsync(detail);
            return MapToResponse(created);
        }

        public async Task<StockDetailResponse> UpdateAsync(int id, CreateStockDetailRequest request)
        {
            var detail = await _repository.GetByIdAsync(id);
            if (detail == null)
                throw new ArgumentException($"StockDetail with id {id} not found");

            detail.SystemQuantity = request.SystemQuantity;
            detail.ActualQuantity = request.ActualQuantity;
            detail.Difference = request.ActualQuantity - request.SystemQuantity;
            detail.HandlingProposal = request.HandlingProposal;

            var updated = await _repository.UpdateAsync(detail);
            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> DeleteByStockCheckIdAsync(int stockCheckId)
        {
            return await _repository.DeleteByStockCheckIdAsync(stockCheckId);
        }

        private static StockDetailResponse MapToResponse(StockCheckDetail detail)
        {
            return new StockDetailResponse
            {
                Id = detail.Id,
                StockCheckId = detail.StockCheckId,
                MaterialId = detail.MaterialId,
                SystemQuantity = detail.SystemQuantity,
                ActualQuantity = detail.ActualQuantity,
                Difference = detail.Difference,
                HandlingProposal = detail.HandlingProposal,
                RecordedCheck = detail.RecordedCheck,
                Status = detail.Status
            };
        }
    }
}
