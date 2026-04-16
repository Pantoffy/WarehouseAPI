using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs.ExportReceiptDTOs
{
    public class ExportReceiptResponse
    {
        public int Id { get; set; }

        public required string Code { get; set; }
        public required string ReceiptNumber { get; set; }
        public DateTime ExportDate { get; set; }

        public int WarehouseId { get; set; }
        public required string ReceiverName { get; set; }
        public required string Reason { get; set; }

        public string? DocumentNo { get; set; }
        public decimal? TotalAmount { get; set; }

        public required string Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public Warehouse? Warehouse { get; set; }
        public ICollection<ExportReceiptDetailResponse>? ExportReceiptDetails { get; set; }
    }
}
