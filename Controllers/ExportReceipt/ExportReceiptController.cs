using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.ExportReceiptDTOs;
using WarehouseAPI.Services.ExportReceipt;
using WarehouseAPI.Services.Inventory;

namespace WarehouseAPI.Controllers.ExportReceipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportReceiptController(IExportReceiptService service, IInventoryUpdateService inventoryUpdateService) : ControllerBase
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
                if (!updated)
                    return NotFound("Không tìm thấy phiếu xuất với Id đã cho.");

                // If status changed to "Approved", update inventory (trừ hàng)
                if (exportReceipt.Status?.Equals("Approved", StringComparison.OrdinalIgnoreCase) == true)
                {
                    await inventoryUpdateService.UpdateInventoryOnExportApprovedAsync(id);
                }

                // If status changed to "Cancelled", revert inventory (hoàn hàng)
                // Hỗ trợ: Cancelled, Cancel, Hủy, Đã hủy, Bị hủy
                var cancelledStatuses = new[] { "Cancelled", "Cancel", "Hủy", "Đã hủy", "Bị hủy" };
                if (cancelledStatuses.Any(s => exportReceipt.Status?.Equals(s, StringComparison.OrdinalIgnoreCase) == true))
                {
                    await inventoryUpdateService.UpdateInventoryOnExportCancelledAsync(id);
                }

                return Ok("Cập nhật thành công");
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
