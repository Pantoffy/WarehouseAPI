using System.ComponentModel.DataAnnotations;
using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs.ImportReceiptDTOs
{
    public class CreateImportReceiptRequest
    {
        [Required(ErrorMessage = "Mã phiếu nhập là bắt buộc")]
        [MinLength(1, ErrorMessage = "Mã phiếu nhập không được để trống")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Số phiếu nhập là bắt buộc")]
        [MinLength(1, ErrorMessage = "Số phiếu nhập không được để trống")]
        public required string ReceiptNumber { get; set; }

        [Required(ErrorMessage = "Thời gian nhập là bắt buộc")]
        public DateTime ImportTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã nhà cung cấp không hợp lệ")]
        public int SupplierId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã kho không hợp lệ")]
        public int WarehouseId { get; set; }

        public string? SupplierInvoiceNo { get; set; }
        public string? DocumentNo { get; set; }

        public decimal? TotalAmount { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [MinLength(1, ErrorMessage = "Trạng thái không được để trống")]
        public required string Status { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Phiếu nhập phải có ít nhất một chi tiết")]
        [MinLength(1, ErrorMessage = "Phiếu nhập phải có ít nhất một chi tiết")]
        public ImportReceiptDetail[]? ImportReceiptDetails { get; set; }
    }
}
