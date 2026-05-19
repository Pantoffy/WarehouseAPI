namespace WarehouseAPI.DTOs.StockDTOs
{
    using WarehouseAPI.DTOs.WarehouseDTOs;

    public class StockResponse
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CheckTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedTime { get; set; }

        // Navigation properties
        public WarehouseResponse? Warehouse { get; set; }
        public List<StockDetailResponse> StockCheckDetails { get; set; } = new();
        public List<StockTeamResponse> Teams { get; set; } = new();
    }
}
