namespace WarehouseAPI.DTOs.StockDTOs
{
    public class CreateStockTeamRequest
    {
        public int StockCheckId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string? Note { get; set; }
    }
}
