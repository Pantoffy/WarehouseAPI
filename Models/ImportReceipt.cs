namespace WarehouseAPI.Models
{
    public class ImportReceipt
    {
        public int Id { get; set; }

        public required string Code { get; set; }
        public required string ReceiptNumber { get; set; }
        public DateTime ImportTime { get; set; }

        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }

        public string? SupplierInvoiceNo { get; set; }
        public string? DocumentNo { get; set; }

        public decimal? TotalAmount { get; set; }
        public required string Status { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Supplier? Supplier { get; set; }
        public Warehouse? Warehouse { get; set; }
        public ICollection<ImportReceiptDetail>? ImportReceiptDetails { get; set; }
    }
}
