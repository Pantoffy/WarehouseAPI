namespace WarehouseAPI.DTOs.StockDTOs
{
    public class UpdateStockRequest
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
    }
}
