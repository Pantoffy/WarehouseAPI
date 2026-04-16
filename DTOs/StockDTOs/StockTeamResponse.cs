namespace WarehouseAPI.DTOs.StockDTOs
{
    public class StockTeamResponse
    {
        public int Id { get; set; }
        public int StockCheckId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string? Note { get; set; }
    }
}
