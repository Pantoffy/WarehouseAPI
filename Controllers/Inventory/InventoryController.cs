using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.InventoryDTOs;
using WarehouseAPI.Services.Inventory;

namespace WarehouseAPI.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IInventoryService service) : ControllerBase
    {
        //hien danh sach ton kho
        [Route(InventoryRouter.GetAllInventories), HttpGet]
        public async Task<ActionResult<List<InventoryResponse>>> GetInventories()
            => Ok(await service.GetAllInventoryAsync());

        [Route(InventoryRouter.GetInventoryById), HttpGet("{id}")]
        public async Task<ActionResult<InventoryResponse?>> GetInventory(int id)
        {
            var inventory = await service.GetInventoryByIdAsync(id);
            return inventory is null ? NotFound("Không tìm thấy tồn kho với Id đã cho.") : Ok(inventory);
        }

        //them moi ton kho
        [Route(InventoryRouter.AddInventory), HttpPost]
        public async Task<ActionResult<InventoryResponse>> AddInventory(CreateInventoryRequest inventory)
        {
            try
            {
                var createdInventory = await service.AddInventoryAsync(inventory);
                return CreatedAtAction(nameof(GetInventory), new { id = createdInventory.Id }, createdInventory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //cap nhat thong tin ton kho
        [Route(InventoryRouter.UpdateInventory), HttpPut("{id}")]
        public async Task<ActionResult> UpdateInventory(int id, UpdateInventoryRequest inventory)
        {
            try
            {
                var updated = await service.UpdateInventoryByIdAsync(id, inventory);
                return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy tồn kho với Id đã cho.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //xoa ton kho
        [Route(InventoryRouter.DeleteInventory), HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInventory(int id)
        {
            var deleted = await service.DeleteInventoryByIdAsync(id);
            return deleted ? Ok("Xóa tồn kho thành công") : NotFound("Không tìm thấy tồn kho với Id đã cho.");
        }
    }
}
