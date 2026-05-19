using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Entities;
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
    public DbSet<StockCheck> StockCheck => Set<StockCheck>();
    public DbSet<StockCheckDetail> StockCheckDetail => Set<StockCheckDetail>();
    public DbSet<StockCheckTeam> StockCheckTeam => Set<StockCheckTeam>();

    public DbSet<User> AppUser {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Disable SQL OUTPUT clause for tables that may have database triggers
        modelBuilder.Entity<ImportReceipt>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<ImportReceiptDetail>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<ExportReceipt>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<ExportReceiptDetail>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<PurchaseOrder>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<PurchaseOrderDetail>().ToTable(tb => tb.UseSqlOutputClause(false));
        modelBuilder.Entity<Inventory>().ToTable(tb => tb.UseSqlOutputClause(false));

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

        // Configure foreign key for StockCheckDetail.WarehouseId
        modelBuilder.Entity<StockCheckDetail>()
            .HasOne(scd => scd.Warehouse)
            .WithMany()
            .HasForeignKey(scd => scd.WarehouseId)
            .HasConstraintName("FK_SCD_Warehouse");

        // Configure computed column for StockCheckDetail.Difference
        modelBuilder.Entity<StockCheckDetail>()
            .Property(scd => scd.Difference)
            .HasComputedColumnSql("[ActualQuantity] - [SystemQuantity]")
            .ValueGeneratedOnAddOrUpdate();

        // Configure foreign key for StockCheckTeam.StockCheckId
        modelBuilder.Entity<StockCheckTeam>()
            .HasOne(sct => sct.StockCheck)
            .WithMany(sc => sc.Teams)
            .HasForeignKey(sct => sct.StockCheckId)
            .HasConstraintName("FK_SCT_Check");
    }
}