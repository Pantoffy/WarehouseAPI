using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.Controllers.Material;
using WarehouseAPI.DTOs.MaterialDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Services.Material;

namespace WarehouseAPI.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController(IMaterialService service) : ControllerBase
    {
        //hien danh sach 
        [Route(MaterialRouter.GetAllMaterials), HttpGet]
        public async Task<ActionResult<List<MaterialResponse>>> GetMaterials()
            => Ok(await service.GetAllMaterialAsync());
        [Route(MaterialRouter.GetMaterialById), HttpGet("{id}")]
        public async Task<ActionResult<MaterialResponse?>> GetMaterial(int id)
        {
            var Material = await service.GetMaterialByIdAsync(id);
            return Material is null ? NotFound("Không tìm thấy nguyên liệu với Id đã cho.") : Ok(Material);
        }
        //them moi 
        [Route(MaterialRouter.AddMaterial), HttpPost]
        public async Task<ActionResult<MaterialResponse>> AddMaterial(CreateMaterialRequest Material)
        {
            try
            {
                var createdMaterial = await service.AddMaterialAsync(Material);
                return CreatedAtAction(nameof(GetMaterial), new { id = createdMaterial.Id }, createdMaterial);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //cap nhat thong tin 
        [Route(MaterialRouter.UpdateMaterial), HttpPut("{id}")]
        public async Task<ActionResult> UpdateMaterial(int id, UpdateMaterialRequest Material)
        {
            try
            {
                var updated = await service.UpdateMaterialByIdAsync(id, Material);
                return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy nguyên liệu với Id đã cho.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //xoa 
        [Route(MaterialRouter.DeleteMaterial), HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaterial(int id)
        {
            var deleted = await service.DeleteMaterialByIdAsync(id);
            return deleted ? Ok("Xóa nguyên liệu thành công") : NotFound("Không tìm thấy nguyên liệu với Id đã cho.");
        }
    }
}
