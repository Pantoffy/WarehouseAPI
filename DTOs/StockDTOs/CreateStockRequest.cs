namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Note { get; set; }
    }
}
