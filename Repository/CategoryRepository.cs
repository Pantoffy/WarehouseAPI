using WarehouseAPI.DTOs;

namespace WarehouseAPI.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetAssetCategoriesAsync();
        Task<List<Category>> GetMaterialCategoriesAsync();
    }

    public class CategoryRepository : ICategoryRepository
    {
        private static readonly List<Category> Categories = new()
        {
            // Material Categories (for food/kitchen items)
                new Category { Id = 1, CategoryName = "Thịt", AssetOnly = false },
                new Category { Id = 2, CategoryName = "Hải sản", AssetOnly = false },
                new Category { Id = 3, CategoryName = "Rau củ quả", AssetOnly = false },
                new Category { Id = 4, CategoryName = "Gia vị", AssetOnly = false },
                new Category { Id = 5, CategoryName = "Dầu mỡ", AssetOnly = false },
                new Category { Id = 6, CategoryName = "Lương thực", AssetOnly = false },
                new Category { Id = 7, CategoryName = "Sữa & Trứng", AssetOnly = false },
                new Category { Id = 8, CategoryName = "Đồ uống", AssetOnly = false },
                new Category { Id = 9, CategoryName = "Đồ khô", AssetOnly = false },
                new Category { Id = 10, CategoryName = "Vật dụng", AssetOnly = false },
                new Category { Id = 11, CategoryName = "Vệ sinh", AssetOnly = false },
                new Category { Id = 12, CategoryName = "Tráng miệng", AssetOnly = false },
            
                // Asset Categories
                new Category { Id = 13, CategoryName = "Nội thất", AssetOnly = true },
                new Category { Id = 14, CategoryName = "Thiết bị", AssetOnly = true },
                new Category { Id = 15, CategoryName = "Công cụ", AssetOnly = true },
                new Category { Id = 16, CategoryName = "Cơ sở vật chất", AssetOnly = true }
        };

        public Task<List<Category>> GetAllCategoriesAsync()
        {
            return Task.FromResult(Categories);
        }

        public Task<Category?> GetCategoryByIdAsync(int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(category);
        }

        public Task<List<Category>> GetAssetCategoriesAsync()
        {
            var assetCategories = Categories.Where(c => c.AssetOnly).ToList();
            return Task.FromResult(assetCategories);
        }

        public Task<List<Category>> GetMaterialCategoriesAsync()
        {
            var materialCategories = Categories.Where(c => !c.AssetOnly).ToList();
            return Task.FromResult(materialCategories);
        }
    }
}
