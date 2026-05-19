namespace WarehouseAPI.DTOs
{
    public static class UnitMapping
    {
        // Dictionary<UnitId, (UnitName, AllowDecimal, BaseUnitId, FactorToBase)>
        private static readonly Dictionary<int, (string name, bool allowDecimal, int? baseUnitId, decimal factorToBase)> UnitMap = new()
        {
            // Weight
            { 1, ("kg", true, 2, 1000m) },
            { 2, ("g", true, null, 1m) },

            // Volume
            { 3, ("lít", true, 4, 1000m) },
            { 4, ("ml", true, null, 1m) },

            // Length
            { 5, ("mét", true, 6, 100m) },
            { 6, ("cm", true, null, 1m) },

            // Count / package units
            { 7, ("cái", false, null, 1m) },
            { 8, ("chiếc", false, null, 1m) },
            { 9, ("bộ", false, null, 1m) },
            { 10, ("con", false, null, 1m) },
            { 11, ("quả", false, null, 1m) },
            { 12, ("hộp", false, null, 1m) },
            { 13, ("gói", false, null, 1m) },
            { 14, ("túi", false, null, 1m) },
            { 15, ("thùng", false, null, 1m) },
            { 16, ("khay", false, null, 1m) },
            { 17, ("chai", false, null, 1m) },
            { 18, ("lon", false, null, 1m) },
            { 19, ("bịch", false, null, 1m) },
            { 20, ("bó", false, null, 1m) },
            { 21, ("cuộn", false, null, 1m) },
            { 22, ("hũ/lọ", false, null, 1m) },
            { 23, ("can", false, null, 1m) },
            { 24, ("cái", false, null, 1m) }
        };

        public static string GetUnitName(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) ? unit.name : "Unknown";
        }

        public static bool IsDecimalAllowed(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) && unit.allowDecimal;
        }

        public static bool IsValidUnit(int unitId)
        {
            return UnitMap.ContainsKey(unitId);
        }

        public static bool HasBaseUnit(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) && unit.baseUnitId.HasValue;
        }

        public static int? GetBaseUnitId(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) ? unit.baseUnitId : null;
        }

        public static decimal GetFactorToBase(int unitId)
        {
            return UnitMap.TryGetValue(unitId, out var unit) ? unit.factorToBase : 1m;
        }
    }
}
