namespace WarehouseAPI.DTOs
{
    public static class CategoryMapping
    {
        private static readonly Dictionary<int, string> CategoryMap = new()
        {
            { 1, "Thịt" },
            { 2, "Hải sản" },
            { 3, "Rau củ quả" },
            { 4, "Gia vị" },
            { 5, "Dầu mỡ" },
            { 6, "Lương thực" },
            { 7, "Sữa & Trứng" },
            { 8, "Đồ uống" },
            { 9, "Đồ khô" },
            { 10, "Vật dụng" },
            { 11, "Vệ sinh" },
            { 12, "Tráng miệng" }
        };

        public static string GetCategoryName(int categoryId)
        {
            return CategoryMap.TryGetValue(categoryId, out var name) ? name : "Unknown";
        }
    }
}
