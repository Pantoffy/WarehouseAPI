using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.ImportReceiptDTOs;
using WarehouseAPI.Services.ImportReceipt;

namespace WarehouseAPI.Controllers.ImportReceipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportReceiptController(IImportReceiptService service) : ControllerBase
    {
        //hien danh sach phieu nhap
        [Route(ImportReceiptRouter.GetAllImportReceipts), HttpGet]
        public async Task<ActionResult<List<ImportReceiptResponse>>> GetImportReceipts()
            => Ok(await service.GetAllImportReceiptAsync());

        [Route(ImportReceiptRouter.GetImportReceiptById), HttpGet("{id}")]
        public async Task<ActionResult<ImportReceiptResponse?>> GetImportReceipt(int id)
        {
            var importReceipt = await service.GetImportReceiptByIdAsync(id);
            return importReceipt is null ? NotFound("Không tìm thấy phiếu nhập với Id đã cho.") : Ok(importReceipt);
        }

        //them moi phieu nhap
        [Route(ImportReceiptRouter.AddImportReceipt), HttpPost]
        public async Task<ActionResult<ImportReceiptResponse>> AddImportReceipt(CreateImportReceiptRequest importReceipt)
        {
            try
            {
                var createdImportReceipt = await service.AddImportReceiptAsync(importReceipt);
                return CreatedAtAction(nameof(GetImportReceipt), new { id = createdImportReceipt.Id }, createdImportReceipt);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //cap nhat thong tin phieu nhap
        [Route(ImportReceiptRouter.UpdateImportReceipt), HttpPut("{id}")]
        public async Task<ActionResult> UpdateImportReceipt(int id, UpdateImportReceiptRequest importReceipt)
        {
            try
            {
                var updated = await service.UpdateImportReceiptByIdAsync(id, importReceipt);
                return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy phiếu nhập với Id đã cho.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //xoa phieu nhap
        [Route(ImportReceiptRouter.DeleteImportReceipt), HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImportReceipt(int id)
        {
            var deleted = await service.DeleteImportReceiptByIdAsync(id);
            return deleted ? Ok("Xóa phiếu nhập thành công") : NotFound("Không tìm thấy phiếu nhập với Id đã cho.");
        }
    }
}
