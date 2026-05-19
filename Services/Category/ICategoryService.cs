using WarehouseAPI.DTOs;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Category
{
    public interface ICategoryService
    {
        Task<List<WarehouseAPI.DTOs.Category>> GetAllCategoriesAsync();
        Task<WarehouseAPI.DTOs.Category?> GetCategoryByIdAsync(int id);
        Task<List<WarehouseAPI.DTOs.Category>> GetAssetCategoriesAsync();
        Task<List<WarehouseAPI.DTOs.Category>> GetMaterialCategoriesAsync();
    }

    public class CategoryService : ICategoryService
    {
        private readonly IUOW _uow;

        public CategoryService(IUOW uow)
        {
            _uow = uow;
        }

        public async Task<List<WarehouseAPI.DTOs.Category>> GetAllCategoriesAsync()
        {
            return await _uow.CategoryRepository.GetAllCategoriesAsync();
        }

        public async Task<WarehouseAPI.DTOs.Category?> GetCategoryByIdAsync(int id)
        {
            return await _uow.CategoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task<List<WarehouseAPI.DTOs.Category>> GetAssetCategoriesAsync()
        {
            return await _uow.CategoryRepository.GetAssetCategoriesAsync();
        }

        public async Task<List<WarehouseAPI.DTOs.Category>> GetMaterialCategoriesAsync()
        {
            return await _uow.CategoryRepository.GetMaterialCategoriesAsync();
        }
    }
}
