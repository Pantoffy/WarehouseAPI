using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Models;

namespace WarehouseAPI.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Warehouse> Warehouse => Set<Warehouse>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<ImportReceipt> ImportReceipt => Set<ImportReceipt>();
    public DbSet<ImportReceiptDetail> ImportReceiptDetail => Set<ImportReceiptDetail>();
    public DbSet<ExportReceipt> ExportReceipt => Set<ExportReceipt>();
    public DbSet<ExportReceiptDetail> ExportReceiptDetail => Set<ExportReceiptDetail>();
    public DbSet<PurchaseOrder> PurchaseOrder => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetail => Set<PurchaseOrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure unique constraint for Inventory (warehouseId, materialId)
        modelBuilder.Entity<Inventory>()
            .HasIndex(i => new { i.WarehouseId, i.MaterialId })
            .IsUnique()
            .HasDatabaseName("UQInventory");

        // Configure unique constraint for ImportReceiptDetail (importReceiptId, materialId)
        modelBuilder.Entity<ImportReceiptDetail>()
            .HasIndex(ird => new { ird.ImportReceiptId, ird.MaterialId })
            .IsUnique()
            .HasDatabaseName("UQImportReceiptDetail");

        // Configure unique constraint for ExportReceiptDetail (exportReceiptId, materialId)
        modelBuilder.Entity<ExportReceiptDetail>()
            .HasIndex(erd => new { erd.ExportReceiptId, erd.MaterialId })
            .IsUnique()
            .HasDatabaseName("UQExportReceiptDetail");

        // Configure unique constraint for PurchaseOrderDetail (purchaseOrderId, materialId)
        modelBuilder.Entity<PurchaseOrderDetail>()
            .HasIndex(pod => new { pod.PurchaseOrderId, pod.MaterialId })
            .IsUnique()
            .HasDatabaseName("UQPurchaseOrderDetail");

        // Configure computed column for ImportReceiptDetail.Amount
        modelBuilder.Entity<ImportReceiptDetail>()
            .Property(ird => ird.Amount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]")
            .ValueGeneratedOnAddOrUpdate();

        // Configure computed column for ExportReceiptDetail.Amount
        modelBuilder.Entity<ExportReceiptDetail>()
            .Property(erd => erd.Amount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]")
            .ValueGeneratedOnAddOrUpdate();

        // Configure computed column for PurchaseOrderDetail.Amount
        modelBuilder.Entity<PurchaseOrderDetail>()
            .Property(pod => pod.Amount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]")
            .ValueGeneratedOnAddOrUpdate();
    }
}