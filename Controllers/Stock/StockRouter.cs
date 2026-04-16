namespace WarehouseAPI.Controllers.Stock
{
    public class StockRouter
    {
        public const string GetAllStocks = "List";
        public const string GetStockById = "Get";
        public const string AddStock = "Add";
        public const string UpdateStock = "Update";
        public const string DeleteStock = "Delete";
        public const string GetStocksByWarehouse = "Warehouse";

        // Stock Team routes
        public const string GetStockTeams = "Teams";
        public const string AddStockTeam = "Team/Add";
    }
}
