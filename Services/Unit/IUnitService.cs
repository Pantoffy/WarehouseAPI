using WarehouseAPI.DTOs.UnitDTOs;

namespace WarehouseAPI.Services.Unit
{
    public interface IUnitService
    {
        Task<List<UnitResponse>> GetAllUnitsAsync();
        Task<UnitResponse?> GetUnitByIdAsync(int id);
    }
}
