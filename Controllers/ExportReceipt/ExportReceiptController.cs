using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WarehouseAPI.DTOs.ExportReceiptDTOs;
using WarehouseAPI.Services.ExportReceipt;
using WarehouseAPI.Services.Inventory;
using WarehouseAPI.Services.Auth;

namespace WarehouseAPI.Controllers.ExportReceipt
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportReceiptController(
        IExportReceiptService service,
        IInventoryUpdateService inventoryUpdateService,
        IUserService userService) : ControllerBase
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
                // Auto-fill createdBy from JWT token
                exportReceipt.CreatedBy = userService.GetUsername(User);

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
                var current = await service.GetExportReceiptByIdAsync(id);
                if (current is null)
                    return NotFound("Không tìm thấy phiếu xuất với Id đã cho.");

                var previousStatus = current.Status ?? string.Empty;
                var nextStatus = exportReceipt.Status ?? string.Empty;

                var approvedStatuses = new[] { "Đã xác nhận", "Approved", "Đã duyệt", "Hoàn thành" };
                var cancelledStatuses = new[] { "Đã hủy", "Cancelled", "Cancel", "Hủy", "Bị hủy" };

                var wasApproved = approvedStatuses.Any(s => previousStatus.Equals(s, StringComparison.OrdinalIgnoreCase));
                var isApproved = approvedStatuses.Any(s => nextStatus.Equals(s, StringComparison.OrdinalIgnoreCase));
                var isCancelled = cancelledStatuses.Any(s => nextStatus.Equals(s, StringComparison.OrdinalIgnoreCase));

                // Auto-fill ApprovedBy and ApprovedAt when transitioning to approved status
                if (!wasApproved && isApproved)
                {
                    exportReceipt.ApprovedBy = userService.GetUsername(User);
                    exportReceipt.ApprovedAt = DateTime.Now;
                }

                var updated = await service.UpdateExportReceiptByIdAsync(id, exportReceipt);
                if (!updated)
                    return NotFound("Không tìm thấy phiếu xuất với Id đã cho.");

                if (!wasApproved && isApproved)
                {
                    await inventoryUpdateService.UpdateInventoryOnExportApprovedAsync(id);
                }
                else if (wasApproved && isCancelled)
                {
                    await inventoryUpdateService.UpdateInventoryOnExportCancelledAsync(id);
                }

                return Ok("Cập nhật thành công");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //xoa phieu xuat
        [Route(ExportReceiptRouter.DeleteExportReceipt), HttpDelete("{id}")]
        [Authorize(Roles = "Quản lý kho")]
        public async Task<ActionResult> DeleteExportReceipt(int id)
        {
            var deleted = await service.DeleteExportReceiptByIdAsync(id);
            return deleted ? Ok("Xóa phiếu xuất thành công") : NotFound("Không tìm thấy phiếu xuất với Id đã cho.");
        }
    }
}
