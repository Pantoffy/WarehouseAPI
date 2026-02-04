namespace WarehouseAPI.Models
{
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public int MaterialId { get; set; }
        public int UnitId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Amount { get; set; }

        public string? Note { get; set; }

        // Navigation properties
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Material? Material { get; set; }
    }
}
