using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WarehouseAPI.Data;
using WarehouseAPI.Repository;
using WarehouseAPI.Services.Supplier;
using WarehouseAPI.Services.Material;
using WarehouseAPI.Services.Warehouse;
using WarehouseAPI.Services.Inventory;
using WarehouseAPI.Services.ImportReceipt;
using WarehouseAPI.Services.ExportReceipt;
using WarehouseAPI.Services.PurchaseOrder;

var builder = WebApplication.CreateBuilder(args);

// =======================
// SERVICES
// =======================

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Dependency Injection
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IImportReceiptService, ImportReceiptService>();
builder.Services.AddScoped<IExportReceiptService, ExportReceiptService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IUOW, UOW>();

var app = builder.Build();

// =======================
// MIDDLEWARE PIPELINE
// =======================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRouting();                 // ✅ BẮT BUỘC – FIX CORS DELETE

app.UseCors("AllowFrontend");     // ✅ SAU routing

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
