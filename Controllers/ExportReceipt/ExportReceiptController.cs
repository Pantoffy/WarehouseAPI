using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.ExportReceiptDTOs;
using WarehouseAPI.Services.ExportReceipt;

namespace WarehouseAPI.Controllers.ExportReceipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportReceiptController(IExportReceiptService service) : ControllerBase
    {
        //hien danh sach phieu xuat
        [Route(ExportReceiptRouter.GetAllExportReceipts), HttpGet]
        public async Task<ActionResult<List<ExportReceiptResponse>>> GetExportReceipts()
            => Ok(await service.GetAllExportReceiptAsync());

        [Route(ExportReceiptRouter.GetExportReceiptById), HttpGet("{id}")]
        public async Task<ActionResult<ExportReceiptResponse?>> GetExportReceipt(int id)
        {
            var exportReceipt = await service.GetExportReceiptByIdAsync(id);
            return exportReceipt is null ? NotFound("Không tìm thấy phiếu xuất với Id đã cho.") : Ok(exportReceipt);
        }

        //them moi phieu xuat
        [Route(ExportReceiptRouter.AddExportReceipt), HttpPost]
        public async Task<ActionResult<ExportReceiptResponse>> AddExportReceipt(CreateExportReceiptRequest exportReceipt)
        {
            try
            {
                var createdExportReceipt = await service.AddExportReceiptAsync(exportReceipt);
                return CreatedAtAction(nameof(GetExportReceipt), new { id = createdExportReceipt.Id }, createdExportReceipt);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //cap nhat thong tin phieu xuat
        [Route(ExportReceiptRouter.UpdateExportReceipt), HttpPut("{id}")]
        public async Task<ActionResult> UpdateExportReceipt(int id, UpdateExportReceiptRequest exportReceipt)
        {
            try
            {
                var updated = await service.UpdateExportReceiptByIdAsync(id, exportReceipt);
                return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy phiếu xuất với Id đã cho.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //xoa phieu xuat
        [Route(ExportReceiptRouter.DeleteExportReceipt), HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExportReceipt(int id)
        {
            var deleted = await service.DeleteExportReceiptByIdAsync(id);
            return deleted ? Ok("Xóa phiếu xuất thành công") : NotFound("Không tìm thấy phiếu xuất với Id đã cho.");
        }
    }
}
