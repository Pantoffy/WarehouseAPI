namespace WarehouseAPI.Models
{
    /// <summary>
    /// Persisted notification record (event-driven: import/export submit, PO delivery, stock check).
    /// Dynamic alerts (low stock, approaching delivery) are generated on-the-fly and NOT stored here.
    /// </summary>
    public class AppNotification
    {
        public int Id { get; set; }

        /// <summary>
        /// Normalized type enum: IMPORT | EXPORT | PURCHASE_ORDER | STOCK_CHECK | LOW_STOCK
        /// </summary>
        public string Type { get; set; } = "";

        public string Title { get; set; } = "";
        public string Message { get; set; } = "";

        /// <summary>
        /// Frontend route to navigate when clicked, e.g. "/nhap-kho?id=5"
        /// </summary>
        public string? TargetUrl { get; set; }

        public string Priority { get; set; } = "normal"; // normal | high | critical

        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
