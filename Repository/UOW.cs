using WarehouseAPI.Data;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Repository
{
    public class UOW : IUOW
    {
        private readonly AppDbContext _context;
        public ISupplierRepository SupplierRepository { get; private set; }
        public IMaterialRepository MaterialRepository { get; private set; }
        public IWarehouseRepository WarehouseRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IImportReceiptRepository ImportReceiptRepository { get; private set; }
        public IExportReceiptRepository ExportReceiptRepository { get; private set; }
        public IPurchaseOrderRepository PurchaseOrderRepository { get; private set; }

        public UOW(AppDbContext context)
        {
            _context = context;
            SupplierRepository = new SupplierRepository(_context);
            MaterialRepository = new MaterialRepository(_context);
            WarehouseRepository = new WarehouseRepository(_context);
            InventoryRepository = new InventoryRepository(_context);
            ImportReceiptRepository = new ImportReceiptRepository(_context);
            ExportReceiptRepository = new ExportReceiptRepository(_context);
            PurchaseOrderRepository = new PurchaseOrderRepository(_context);
        }
    }
}
    public interface IUOW
    {
        ISupplierRepository SupplierRepository { get; }
        IMaterialRepository MaterialRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IImportReceiptRepository ImportReceiptRepository { get; }
        IExportReceiptRepository ExportReceiptRepository { get; }
        IPurchaseOrderRepository PurchaseOrderRepository { get; }
    }
