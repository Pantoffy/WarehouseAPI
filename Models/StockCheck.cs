namespace WarehouseAPI.Models
{
    public class StockCheck
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CheckTime { get; set; }

        public string? CreatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        // Navigation properties
        public Warehouse? Warehouse { get; set; }
        public ICollection<StockCheckTeam>? Teams { get; set; }
    }
}
