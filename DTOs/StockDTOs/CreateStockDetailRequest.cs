namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockDetailRequest
    {
        public int StockCheckId { get; set; }
        public int MaterialId { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public string? HandlingProposal { get; set; }
    }
}
