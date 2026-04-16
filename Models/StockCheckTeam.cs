namespace WarehouseAPI.Models
{
    public class StockCheckTeam
    {
        public int Id { get; set; }
        public int StockCheckId { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Note { get; set; }

        // Navigation properties
        public StockCheck? StockCheck { get; set; }
    }
}
