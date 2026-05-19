using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WarehouseAPI.DTOs.ImportReceiptDTOs;
using WarehouseAPI.Services.ImportReceipt;
using WarehouseAPI.Services.Inventory;
using WarehouseAPI.Services.Auth;

namespace WarehouseAPI.Controllers.ImportReceipt
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImportReceiptController(IImportReceiptService service, IInventoryUpdateService inventoryUpdateService, IUserService userService) : ControllerBase
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
                // Auto-fill createdBy from JWT token
                importReceipt.CreatedBy = userService.GetUsername(User);

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
                var current = await service.GetImportReceiptByIdAsync(id);
                if (current is null)
                    return NotFound("Không tìm thấy phiếu nhập với Id đã cho.");

                var previousStatus = current.Status ?? string.Empty;
                var nextStatus = importReceipt.Status ?? string.Empty;

                var approvedStatuses = new[] { "Đã xác nhận", "Approved", "Đã duyệt", "Hoàn thành" };
                var cancelledStatuses = new[] { "Đã hủy", "Cancelled", "Cancel", "Hủy", "Bị hủy" };

                var wasApproved = approvedStatuses.Any(s => previousStatus.Equals(s, StringComparison.OrdinalIgnoreCase));
                var isApproved = approvedStatuses.Any(s => nextStatus.Equals(s, StringComparison.OrdinalIgnoreCase));
                var isCancelled = cancelledStatuses.Any(s => nextStatus.Equals(s, StringComparison.OrdinalIgnoreCase));

                // Auto-fill ApprovedBy and ApprovedAt when transitioning to approved status
                if (!wasApproved && isApproved)
                {
                    importReceipt.ApprovedBy = userService.GetUsername(User);
                    importReceipt.ApprovedAt = DateTime.Now;
                }

                var updated = await service.UpdateImportReceiptByIdAsync(id, importReceipt);
                if (!updated)
                    return NotFound("Không tìm thấy phiếu nhập với Id đã cho.");

                // Only apply inventory mutation on real status transition
                if (!wasApproved && isApproved)
                {
                    await inventoryUpdateService.UpdateInventoryOnImportApprovedAsync(id);
                }
                else if (wasApproved && isCancelled)
                {
                    await inventoryUpdateService.UpdateInventoryOnImportCancelledAsync(id);
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

        //xoa phieu nhap
        [Route(ImportReceiptRouter.DeleteImportReceipt), HttpDelete("{id}")]
        [Authorize(Roles = "Quản lý kho")]
        public async Task<ActionResult> DeleteImportReceipt(int id)
        {
            var deleted = await service.DeleteImportReceiptByIdAsync(id);
            return deleted ? Ok("Xóa phiếu nhập thành công") : NotFound("Không tìm thấy phiếu nhập với Id đã cho.");
        }
    }
}
