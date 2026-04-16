using System.ComponentModel.DataAnnotations;

namespace WarehouseAPI.DTOs.ExportReceiptDTOs
{
    public class UpdateExportReceiptRequest
    {
        [Required(ErrorMessage = "Mã phiếu xuất là bắt buộc")]
        [MinLength(1, ErrorMessage = "Mã phiếu xuất không được để trống")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Số phiếu xuất là bắt buộc")]
        [MinLength(1, ErrorMessage = "Số phiếu xuất không được để trống")]
        public required string ReceiptNumber { get; set; }

        [Required(ErrorMessage = "Ngày xuất là bắt buộc")]
        public DateTime ExportDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã kho không hợp lệ")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [MinLength(1, ErrorMessage = "Tên người nhận không được để trống")]
        public required string ReceiverName { get; set; }

        [Required(ErrorMessage = "Lý do xuất là bắt buộc")]
        [MinLength(1, ErrorMessage = "Lý do xuất không được để trống")]
        public required string Reason { get; set; }

        public string? DocumentNo { get; set; }
        public decimal? TotalAmount { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [MinLength(1, ErrorMessage = "Trạng thái không được để trống")]
        public required string Status { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public UpdateExportReceiptDetailRequest[]? ExportReceiptDetails { get; set; }
    }
}
