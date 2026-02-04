using System.ComponentModel;

namespace WarehouseAPI.Controllers.Material
{

    public class MaterialRouter
    {
        private const string BasePath = "api/materials";
        public const string GetAllMaterials = BasePath;
        public const string GetMaterialById = BasePath + "/{id}";
        public const string AddMaterial = BasePath;
        public const string UpdateMaterial = BasePath + "/{id}";
        public const string DeleteMaterial = BasePath + "/{id}";

    }
}