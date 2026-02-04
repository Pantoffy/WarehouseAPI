using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.MaterialDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Material
{
    public class MaterialService : IMaterialService
    {
        private IUOW UOW;
        public MaterialService(IUOW uow)
        {
            UOW = uow;
        }
        public async Task<MaterialResponse> AddMaterialAsync(CreateMaterialRequest Material)
        {
            return await UOW.MaterialRepository.AddMaterialAsync(Material);
        }

        public async Task<bool> DeleteMaterialByIdAsync(int id)
        {
            return await UOW.MaterialRepository.DeleteMaterialByIdAsync(id);
        }

        public async Task<List<MaterialResponse>> GetAllMaterialAsync()
        {
            return await UOW.MaterialRepository.GetAllMaterialAsync();
        }


        public async Task<MaterialResponse> GetMaterialByIdAsync(int id)
        {
            return await UOW.MaterialRepository.GetMaterialByIdAsync(id);
        }

        public async Task<bool> UpdateMaterialByIdAsync(int id, UpdateMaterialRequest Material)
        {
            return await UOW.MaterialRepository.UpdateMaterialByIdAsync(id, Material);
        }
    }
}
