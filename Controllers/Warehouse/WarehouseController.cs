using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.WarehouseDTOs;
using WarehouseAPI.Services.Warehouse;

namespace WarehouseAPI.Controllers.Warehouse
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController(IWarehouseService service) : ControllerBase
    {
        //hien danh sach kho
        [Route(WarehouseRouter.GetAllWarehouses), HttpGet]
        public async Task<ActionResult<List<WarehouseResponse>>> GetWarehouses()
            => Ok(await service.GetAllWarehouseAsync());

        [Route(WarehouseRouter.GetWarehouseById), HttpGet("{id}")]
        public async Task<ActionResult<WarehouseResponse?>> GetWarehouse(int id)
        {
            var warehouse = await service.GetWarehouseByIdAsync(id);
            return warehouse is null ? NotFound("Không tìm thấy kho với Id đã cho.") : Ok(warehouse);
        }

        //them moi kho
        [Route(WarehouseRouter.AddWarehouse), HttpPost]
        public async Task<ActionResult<WarehouseResponse>> AddWarehouse(CreateWarehouseRequest warehouse)
        {
            var createdWarehouse = await service.AddWarehouseAsync(warehouse);
            return CreatedAtAction(nameof(GetWarehouse), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        //cap nhat thong tin kho
        [Route(WarehouseRouter.UpdateWarehouse), HttpPut("{id}")]
        public async Task<ActionResult> UpdateWarehouse(int id, UpdateWarehouseRequest warehouse)
        {
            var updated = await service.UpdateWarehouseByIdAsync(id, warehouse);
            return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy kho với Id đã cho.");
        }

        //xoa kho
        [Route(WarehouseRouter.DeleteWarehouse), HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWarehouse(int id)
        {
            var deleted = await service.DeleteWarehouseByIdAsync(id);
            return deleted ? Ok("Xóa kho thành công") : NotFound("Không tìm thấy kho với Id đã cho.");
        }
    }
}
