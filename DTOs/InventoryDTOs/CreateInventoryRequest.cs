namespace WarehouseAPI.DTOs.InventoryDTOs
{
    public class CreateInventoryRequest
    {
        public int WarehouseId { get; set; }
        public int MaterialId { get; set; }

        public decimal Quantity { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
