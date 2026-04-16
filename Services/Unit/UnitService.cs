using WarehouseAPI.DTOs.UnitDTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Unit
{
    public class UnitService : IUnitService
    {
        private readonly IUOW _uow;

        public UnitService(IUOW uow)
        {
            _uow = uow;
        }

        public async Task<List<UnitResponse>> GetAllUnitsAsync()
        {
            return await _uow.UnitRepository.GetAllUnitsAsync();
        }

        public async Task<UnitResponse?> GetUnitByIdAsync(int id)
        {
            return await _uow.UnitRepository.GetUnitByIdAsync(id);
        }
    }
}
