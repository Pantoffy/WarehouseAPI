using WarehouseAPI.DTOs.UnitDTOs;

namespace WarehouseAPI.Repository
{
    public interface IUnitRepository
    {
        Task<List<UnitResponse>> GetAllUnitsAsync();
        Task<UnitResponse?> GetUnitByIdAsync(int id);
    }

    public class UnitRepository : IUnitRepository
    {
        private static readonly List<UnitResponse> Units = new()
        {
            new UnitResponse { Id = 1, Name = "kg" },
            new UnitResponse { Id = 2, Name = "g" },
            new UnitResponse { Id = 3, Name = "lít" },
            new UnitResponse { Id = 4, Name = "ml" },
            new UnitResponse { Id = 5, Name = "quả" },
            new UnitResponse { Id = 6, Name = "con" },
            new UnitResponse { Id = 7, Name = "bó" },
            new UnitResponse { Id = 8, Name = "hộp" },
            new UnitResponse { Id = 9, Name = "gói" },
            new UnitResponse { Id = 10, Name = "chai" },
            new UnitResponse { Id = 11, Name = "lon" },
            new UnitResponse { Id = 12, Name = "túi" },
            new UnitResponse { Id = 13, Name = "cuộn" },
            new UnitResponse { Id = 14, Name = "thùng" },
            new UnitResponse { Id = 15, Name = "hũ/lọ" },
            new UnitResponse { Id = 16, Name = "tấm/miếng" },
            new UnitResponse { Id = 17, Name = "cây" },
            new UnitResponse { Id = 18, Name = "bịch" }
        };

        public Task<List<UnitResponse>> GetAllUnitsAsync()
        {
            return Task.FromResult(Units);
        }

        public Task<UnitResponse?> GetUnitByIdAsync(int id)
        {
            var unit = Units.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(unit);
        }
    }
}
