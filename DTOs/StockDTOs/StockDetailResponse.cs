namespace WarehouseAPI.DTOs.StockDTOs
{
    using WarehouseAPI.DTOs.MaterialDTOs;

    public class StockDetailResponse
    {
        public int Id { get; set; }
        public int StockCheckId { get; set; }
        public int MaterialId { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal Difference { get; set; }
        public string? HandlingProposal { get; set; }
        public bool RecordedCheck { get; set; }
        public string? Status { get; set; }

        // Navigation properties
        public MaterialResponse? Material { get; set; }
    }
}
