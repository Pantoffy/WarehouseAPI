using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs.PurchaseOrderDTOs
{
    public class PurchaseOrderDetailResponse
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public int MaterialId { get; set; }
        public int UnitId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Amount { get; set; }

        public string? Note { get; set; }

        public Material? Material { get; set; }
    }
}
