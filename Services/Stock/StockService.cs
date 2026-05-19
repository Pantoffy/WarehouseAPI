using Microsoft.EntityFrameworkCore;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs.MaterialDTOs;
using WarehouseAPI.DTOs.StockDTOs;
using WarehouseAPI.DTOs.WarehouseDTOs;
using StockCheckModel = WarehouseAPI.Models.StockCheck;
using WarehouseAPI.Models;
using WarehouseAPI.Repository;

namespace WarehouseAPI.Services.Stock
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;
        private readonly StockCheckRepository _repository;
        private readonly StockCheckDetailRepository _detailRepository;
        private readonly StockCheckTeamRepository _teamRepository;

        public StockService(
            AppDbContext context,
            StockCheckRepository repository,
            StockCheckDetailRepository detailRepository,
            StockCheckTeamRepository teamRepository)
        {
            _context = context;
            _repository = repository;
            _detailRepository = detailRepository;
            _teamRepository = teamRepository;
        }

        public async Task<StockResponse?> GetByIdAsync(int id)
        {
            var stockCheck = await _repository.GetByIdAsync(id);
            return stockCheck == null ? null : await MapToResponseAsync(stockCheck);
        }

        public async Task<List<StockResponse>> GetAllAsync()
        {
            var stockChecks = await _repository.GetAllAsync();

            var allDetails = await _detailRepository.GetAllAsync();
            var detailsByStockId = allDetails.GroupBy(d => d.StockCheckId).ToDictionary(g => g.Key, g => g.ToList());

            var results = stockChecks.Select(sc => MapToResponse(
                sc,
                detailsByStockId.TryGetValue(sc.Id, out var details) ? details : [],
                sc.Teams?.ToList() ?? []))
                .ToList();
            return results;
        }

        public async Task<List<StockResponse>> GetByWarehouseIdAsync(int warehouseId)
        {
            var stockChecks = await _repository.GetByWarehouseIdAsync(warehouseId);

            var stockCheckIds = stockChecks.Select(sc => sc.Id).ToList();
            var allDetails = await _detailRepository.GetAllAsync();

            var detailsByStockId = allDetails
                .Where(d => stockCheckIds.Contains(d.StockCheckId))
                .GroupBy(d => d.StockCheckId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var results = stockChecks.Select(sc => MapToResponse(
                sc,
                detailsByStockId.TryGetValue(sc.Id, out var details) ? details : [],
                sc.Teams?.ToList() ?? []))
                .ToList();
            return results;
        }

        public async Task<StockResponse> CreateAsync(CreateStockRequest request)
        {
            var duplicateMaterialIds = request.StockCheckDetails
                .GroupBy(d => d.MaterialId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateMaterialIds.Any())
                throw new InvalidOperationException("Chi tiết phiếu kiểm không được trùng vật tư.");

            var warehouseExists = await _context.Warehouse.AnyAsync(w => w.Id == request.WarehouseId);
            if (!warehouseExists)
                throw new InvalidOperationException($"Kho hàng có ID {request.WarehouseId} không tồn tại.");

            var materialIds = request.StockCheckDetails.Select(d => d.MaterialId).Distinct().ToList();
            var existingMaterialIds = await _context.Materials
                .Where(m => materialIds.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync();

            foreach (var materialId in materialIds)
            {
                if (!existingMaterialIds.Contains(materialId))
                    throw new InvalidOperationException($"Vật liệu có ID {materialId} không tồn tại.");
            }

            var executionStrategy = _context.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var stockCheck = new StockCheckModel
                    {
                        Code = request.Code,
                        Name = request.Name,
                        WarehouseId = request.WarehouseId,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        CreatedBy = request.CreatedBy,
                        Note = request.Note,
                        Status = "Nháp",
                        CreatedTime = DateTime.Now
                    };

                    _context.StockCheck.Add(stockCheck);
                    await _context.SaveChangesAsync();

                    var details = request.StockCheckDetails.Select(d => new StockCheckDetail
                    {
                        StockCheckId = stockCheck.Id,
                        MaterialId = d.MaterialId,
                        WarehouseId = stockCheck.WarehouseId,
                        SystemQuantity = d.SystemQuantity,
                        ActualQuantity = d.ActualQuantity,
                        HandlingProposal = d.HandlingProposal,
                        RecordedCheck = true,
                        Status = "Chưa xử lý"
                    }).ToList();

                    var teams = request.Teams.Select(t => new StockCheckTeam
                    {
                        StockCheckId = stockCheck.Id,
                        Name = t.Name,
                        Role = t.Role,
                        Note = t.Note
                    }).ToList();

                    _context.StockCheckDetail.AddRange(details);
                    _context.StockCheckTeam.AddRange(teams);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var created = await _repository.GetByIdAsync(stockCheck.Id);
                    if (created == null)
                        throw new InvalidOperationException("Không thể tải phiếu kiểm vừa tạo.");

                    return await MapToResponseAsync(created);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<StockResponse> UpdateAsync(int id, UpdateStockRequest request)
        {
            var stockCheck = await _repository.GetByIdAsync(id);
            if (stockCheck == null)
                throw new ArgumentException($"Stock with id {id} not found");

            if (!string.IsNullOrEmpty(request.Code))
                stockCheck.Code = request.Code;
            if (!string.IsNullOrEmpty(request.Name))
                stockCheck.Name = request.Name;
            if (request.StartDate.HasValue)
                stockCheck.StartDate = request.StartDate;
            if (request.EndDate.HasValue)
                stockCheck.EndDate = request.EndDate;
            if (!string.IsNullOrEmpty(request.ApprovedBy))
                stockCheck.ApprovedBy = request.ApprovedBy;
            if (!string.IsNullOrEmpty(request.Status))
                stockCheck.Status = request.Status;
            if (request.Status == "Đã duyệt" && !stockCheck.ApprovedAt.HasValue)
                stockCheck.ApprovedAt = DateTime.Now;
            if (!string.IsNullOrEmpty(request.Note))
                stockCheck.Note = request.Note;

            var updated = await _repository.UpdateAsync(stockCheck);
            return await MapToResponseAsync(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _detailRepository.DeleteByStockCheckIdAsync(id);
            await _teamRepository.DeleteByStockCheckIdAsync(id);

            return await _repository.DeleteAsync(id);
        }

        private async Task<StockResponse> MapToResponseAsync(StockCheckModel stockCheck)
        {
            var details = await _detailRepository.GetByStockCheckIdAsync(stockCheck.Id);
            var teams = await _teamRepository.GetByStockCheckIdAsync(stockCheck.Id);
            return MapToResponse(stockCheck, details, teams);
        }

        private StockResponse MapToResponse(StockCheckModel stockCheck, List<StockCheckDetail> details, List<StockCheckTeam> teams)
        {
            return new StockResponse
            {
                Id = stockCheck.Id,
                Code = stockCheck.Code,
                Name = stockCheck.Name,
                WarehouseId = stockCheck.WarehouseId,
                StartDate = stockCheck.StartDate,
                EndDate = stockCheck.EndDate,
                CheckTime = stockCheck.CheckTime,
                CreatedBy = stockCheck.CreatedBy,
                ApprovedBy = stockCheck.ApprovedBy,
                ApprovedAt = stockCheck.ApprovedAt,
                Status = stockCheck.Status,
                Note = stockCheck.Note,
                CreatedTime = stockCheck.CreatedTime,
                Warehouse = stockCheck.Warehouse == null ? null : new WarehouseResponse
                {
                    Id = stockCheck.Warehouse.Id,
                    Code = stockCheck.Warehouse.Code,
                    Name = stockCheck.Warehouse.Name,
                    TypeId = stockCheck.Warehouse.TypeId,
                    Address = stockCheck.Warehouse.Address,
                    Area = stockCheck.Warehouse.Area,
                    ManagerName = stockCheck.Warehouse.ManagerName,
                    ManagerPhone = stockCheck.Warehouse.ManagerPhone,
                    Status = stockCheck.Warehouse.Status,
                    Note = stockCheck.Warehouse.Note,
                    CreatedTime = stockCheck.Warehouse.CreatedTime
                },
                StockCheckDetails = details.Select(d => new StockDetailResponse
                {
                    Id = d.Id,
                    StockCheckId = d.StockCheckId,
                    MaterialId = d.MaterialId,
                    SystemQuantity = d.SystemQuantity,
                    ActualQuantity = d.ActualQuantity,
                    Difference = d.Difference,
                    HandlingProposal = d.HandlingProposal,
                    RecordedCheck = d.RecordedCheck,
                    Status = d.Status,
                    Material = d.Material == null ? null : new MaterialResponse
                    {
                        Id = d.Material.Id,
                        Code = d.Material.Code,
                        Name = d.Material.Name,
                        CategoryId = d.Material.CategoryId,
                        UnitId = d.Material.UnitId,
                        SupplierId = d.Material.SupplierId,
                        Status = d.Material.Status,
                        Note = d.Material.Note,
                        CreatedTime = d.Material.CreatedTime,
                        Supplier = d.Material.Supplier
                    }
                }).ToList(),
                Teams = teams.Select(t => new StockTeamResponse
                {
                    Id = t.Id,
                    StockCheckId = t.StockCheckId,
                    Name = t.Name,
                    Role = t.Role,
                    Note = t.Note
                }).ToList()
            };
        }
    }
}
