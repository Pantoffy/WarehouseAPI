using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using WarehouseAPI.Data;
using WarehouseAPI.Repository;
using WarehouseAPI.Services;
using WarehouseAPI.Services.Supplier;
using WarehouseAPI.Services.Material;
using WarehouseAPI.Services.Warehouse;
using WarehouseAPI.Services.Inventory;
using WarehouseAPI.Services.ImportReceipt;
using WarehouseAPI.Services.ExportReceipt;
using WarehouseAPI.Services.PurchaseOrder;
using WarehouseAPI.Services.Auth;
using WarehouseAPI.Services.Unit;
using WarehouseAPI.Services.Category;
using WarehouseAPI.Services.Stock;
using WarehouseAPI.Services.Calendar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

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
                "http://localhost:5174",
                "https://localhost:5174",
                "http://localhost:3000",
                "https://localhost:3000")
            .WithHeaders("Content-Type", "Authorization", "ngrok-skip-browser-warning")
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
    });
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IImportReceiptService, ImportReceiptService>();
builder.Services.AddScoped<IExportReceiptService, ExportReceiptService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUOW, UOW>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInventoryUpdateService, InventoryUpdateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Stock Services
builder.Services.AddScoped<StockCheckRepository>();
builder.Services.AddScoped<StockCheckDetailRepository>();
builder.Services.AddScoped<StockCheckTeamRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockDetailService, StockDetailService>();
builder.Services.AddScoped<IStockTeamService, StockTeamService>();

// Calendar Service
builder.Services.AddScoped<ICalendarService, CalendarService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseRouting();           

app.UseCors("AllowFrontend");   

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
