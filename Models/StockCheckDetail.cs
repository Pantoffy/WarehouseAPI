namespace WarehouseAPI.Models
{
    public class StockCheckDetail
    {
        public int Id { get; set; }
        public int StockCheckId { get; set; }
        public int MaterialId { get; set; }

        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal Difference { get; set; }

        // New fields
        public int? WarehouseId { get; set; }
        public string? HandlingProposal { get; set; }
        public bool RecordedCheck { get; set; } = true;
        public string? Status { get; set; }

        // Navigation properties
        public StockCheck? StockCheck { get; set; }
        public Material? Material { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
