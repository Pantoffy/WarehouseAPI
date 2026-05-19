namespace WarehouseAPI.DTOs.CalendarDTOs
{
    public class CalendarEventResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public decimal? Quantity { get; set; }
        public string? WarehouseName { get; set; }
        public string? Code { get; set; }
        public string? DetailUrl { get; set; }
    }
}

