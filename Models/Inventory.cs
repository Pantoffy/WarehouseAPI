namespace WarehouseAPI.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public int MaterialId { get; set; }

        public decimal Quantity { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Warehouse? Warehouse { get; set; }
        public Material? Material { get; set; }
    }
}
