using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs;
using WarehouseAPI.Services.Category;

namespace WarehouseAPI.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService service) : ControllerBase
    {
        [Route(CategoryRouter.GetAllCategories), HttpGet]
        public async Task<ActionResult<List<WarehouseAPI.DTOs.Category>>> GetAllCategories()
            => Ok(await service.GetAllCategoriesAsync());

        [Route(CategoryRouter.GetCategoryById), HttpGet("{id}")]
        public async Task<ActionResult<WarehouseAPI.DTOs.Category?>> GetCategoryById(int id)
        {
            var category = await service.GetCategoryByIdAsync(id);
            return category is null ? NotFound("Không tìm thấy danh mục với Id đã cho.") : Ok(category);
        }

        [Route(CategoryRouter.GetAssetCategories), HttpGet]
        public async Task<ActionResult<List<WarehouseAPI.DTOs.Category>>> GetAssetCategories()
            => Ok(await service.GetAssetCategoriesAsync());

        [Route(CategoryRouter.GetMaterialCategories), HttpGet]
        public async Task<ActionResult<List<WarehouseAPI.DTOs.Category>>> GetMaterialCategories()
            => Ok(await service.GetMaterialCategoriesAsync());
    }
}
