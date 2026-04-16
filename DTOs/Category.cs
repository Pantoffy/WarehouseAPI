using WarehouseAPI.Models;

namespace WarehouseAPI.DTOs
{
    public class Category
    {
        public int Id { get; set; }
        public required string CategoryName { get; set; }
    }
}
