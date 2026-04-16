namespace WarehouseAPI.DTOs.PurchaseOrderDTOs
{
    public class UpdatePurchaseOrderDetailRequest
    {
        public int? Id { get; set; } // Null nếu tạo mới, có ID nếu cập nhật

        public int MaterialId { get; set; }
        public int UnitId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Amount { get; set; }

        public string? Note { get; set; }
    }
}
