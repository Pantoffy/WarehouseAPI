using WarehouseAPI.DTOs.StockDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Stock
{
    public class StockTeamService : IStockTeamService
    {
        private readonly StockCheckTeamRepository _repository;

        public StockTeamService(StockCheckTeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<StockTeamResponse?> GetByIdAsync(int id)
        {
            var team = await _repository.GetByIdAsync(id);
            return team == null ? null : MapToResponse(team);
        }

        public async Task<List<StockTeamResponse>> GetAllAsync()
        {
            var teams = await _repository.GetAllAsync();
            return teams.Select(MapToResponse).ToList();
        }

        public async Task<List<StockTeamResponse>> GetByStockCheckIdAsync(int stockCheckId)
        {
            var teams = await _repository.GetByStockCheckIdAsync(stockCheckId);
            return teams.Select(MapToResponse).ToList();
        }

        public async Task<StockTeamResponse> CreateAsync(CreateStockTeamRequest request)
        {
            var team = new StockCheckTeam
            {
                StockCheckId = request.StockCheckId,
                Name = request.Name,
                Role = request.Role,
                Note = request.Note
            };

            var created = await _repository.CreateAsync(team);
            return MapToResponse(created);
        }

        public async Task<StockTeamResponse> UpdateAsync(int id, CreateStockTeamRequest request)
        {
            var team = await _repository.GetByIdAsync(id);
            if (team == null)
                throw new ArgumentException($"StockTeam with id {id} not found");

            team.Name = request.Name;
            team.Role = request.Role;
            team.Note = request.Note;

            var updated = await _repository.UpdateAsync(team);
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

        private static StockTeamResponse MapToResponse(StockCheckTeam team)
        {
            return new StockTeamResponse
            {
                Id = team.Id,
                StockCheckId = team.StockCheckId,
                Name = team.Name,
                Role = team.Role,
                Note = team.Note
            };
        }
    }
}
