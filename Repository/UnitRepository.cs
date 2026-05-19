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
            // Weight
            new UnitResponse { Id = 1, Name = "kg" },
            new UnitResponse { Id = 2, Name = "g" },
            
            // Volume
            new UnitResponse { Id = 3, Name = "lít" },
            new UnitResponse { Id = 4, Name = "ml" },
            
            // Length
            new UnitResponse { Id = 5, Name = "mét" },
            new UnitResponse { Id = 6, Name = "cm" },
            
            // Count
            new UnitResponse { Id = 7, Name = "cái" },
            new UnitResponse { Id = 8, Name = "chiếc" },
            new UnitResponse { Id = 9, Name = "bộ" },
            new UnitResponse { Id = 10, Name = "con" },
            new UnitResponse { Id = 11, Name = "quả" },
            new UnitResponse { Id = 20, Name = "bó" },
            new UnitResponse { Id = 21, Name = "cuộn" },
            
            // Package
            new UnitResponse { Id = 12, Name = "hộp" },
            new UnitResponse { Id = 13, Name = "gói" },
            new UnitResponse { Id = 14, Name = "túi" },
            new UnitResponse { Id = 15, Name = "thùng" },
            new UnitResponse { Id = 16, Name = "khay" },
            new UnitResponse { Id = 17, Name = "chai" },
            new UnitResponse { Id = 18, Name = "lon" },
            new UnitResponse { Id = 19, Name = "bịch" },
            new UnitResponse { Id = 22, Name = "hũ/lọ" },
            new UnitResponse { Id = 23, Name = "can" }
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
