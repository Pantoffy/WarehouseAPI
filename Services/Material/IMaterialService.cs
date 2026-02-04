using WarehouseAPI.DTOs.MaterialDTOs;
using WarehouseAPI.Models;
namespace WarehouseAPI.Services.Material
{
    public interface IMaterialService
    {
        Task<List<MaterialResponse>> GetAllMaterialAsync();
        Task<MaterialResponse> GetMaterialByIdAsync(int id);
        Task<MaterialResponse> AddMaterialAsync(CreateMaterialRequest Material);
        Task<bool> UpdateMaterialByIdAsync(int id, UpdateMaterialRequest Material);
        Task<bool> DeleteMaterialByIdAsync(int id);

    }
}
