using System.ComponentModel;

namespace WarehouseAPI.Controllers.Supplier
{

    public class SupplierRouter
    {
        private const string BasePath = "api/suppliers";
        public const string GetAllSuppliers = BasePath;
        public const string GetSupplierById = BasePath + "/{id}";
        public const string AddSupplier = BasePath;
        public const string UpdateSupplier = BasePath + "/{id}";
        public const string DeleteSupplier = BasePath + "/{id}";

    }
}