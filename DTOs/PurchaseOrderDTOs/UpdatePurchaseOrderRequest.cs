namespace WarehouseAPI.DTOs.PurchaseOrderDTOs
{
    public class UpdatePurchaseOrderRequest
    {
        public required string Code { get; set; }
        public required string PoNumber { get; set; }
        public DateTime OrderDate { get; set; }

        public int SupplierId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public decimal? TotalAmount { get; set; }
        public required string Status { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
