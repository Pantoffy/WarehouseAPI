using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.SupplierDTOs;
using WarehouseAPI.Models;
using WarehouseAPI.Services.Supplier;

namespace WarehouseAPI.Controllers.Supplier
{
    [ApiController]
    public class SupplierController(ISupplierService service) : ControllerBase
    {
        //hien danh sach nha cung cap
        [Route(SupplierRouter.GetAllSuppliers),HttpGet]
        public async Task<ActionResult<List<SupplierResponse>>> GetSuppliers()
            => Ok(await service.GetAllSupplierAsync());
        [Route(SupplierRouter.GetSupplierById),HttpGet("{id}")]
        public async Task<ActionResult<SupplierResponse?>> GetSupplier(int id)
        {
            var supplier = await service.GetSupplierByIdAsync(id);
            return supplier is null ? NotFound("Không tìm thấy nhà cung cấp với Id đã cho.") : Ok(supplier);
        }
        //them moi nha cung cap
        [Route(SupplierRouter.AddSupplier),HttpPost]
        public async Task<ActionResult<SupplierResponse>> AddSupplier(CreateSupplierRequest supplier)
        {
            var createdSupplier = await service.AddSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.Id }, createdSupplier); 
        }
        //cap nhat thong tin nha cung cap
        [Route(SupplierRouter.UpdateSupplier),HttpPut/*("{id}")*/]
        public async Task<ActionResult> UpdateSupplier(int id, UpdateSupplierRequest supplier)
        {
            var updated = await service.UpdateSupplierByIdAsync(id, supplier);
            return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy nhà cung cấp với Id đã cho.");
        }
        //xoa nha cung cap
        [Route(SupplierRouter.DeleteSupplier),HttpDelete/*("{id}")*/]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            var deleted = await service.DeleteSupplierByIdAsync(id);
            return deleted ? Ok("Xóa nhà cung cấp thành công") : NotFound("Không tìm thấy nhà cung cấp với Id đã cho.");
        }
    }
}
