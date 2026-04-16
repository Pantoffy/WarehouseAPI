namespace WarehouseAPI.DTOs
{
    public static class UnitMapping
    {
        // Dictionary<UnitId, (UnitName, AllowDecimal)>
        private static readonly Dictionary<int, (string name, bool allowDecimal)> UnitMap = new()
        {
            { 1, ("kg", true) },
            { 2, ("g", true) },
            { 3, ("lít", true) },
            { 4, ("ml", true) },
            { 5, ("quả", false) },
            { 6, ("con", false) },
            { 7, ("bó", false) },
            { 8, ("hộp", false) },
            { 9, ("gói", false) },
            { 10, ("chai", false) },
            { 11, ("lon", false) },
            { 12, ("túi", false) },
            { 13, ("cuộn", false) },
            { 14, ("thùng", false) },
            { 15, ("hũ/lọ", false) },
            { 16, ("tấm/miếng", false) },
            { 17, ("cây", false) },
            { 18, ("bịch", false) }
        };

        public static string GetUnitName(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) ? unit.name : "Unknown";
        }

        public static bool IsDecimalAllowed(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) && unit.allowDecimal;
        }
    }
}
