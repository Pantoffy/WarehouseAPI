using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WarehouseAPI.DTOs.StockDTOs;
using WarehouseAPI.Services.Stock;
using WarehouseAPI.Services.Auth;

namespace WarehouseAPI.Controllers.Stock
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        private readonly IStockDetailService _detailService;
        private readonly IStockTeamService _teamService;
        private readonly IUserService _userService;

        public StockController(
            IStockService service,
            IStockDetailService detailService,
            IStockTeamService teamService,
            IUserService userService)
        {
            _service = service;
            _detailService = detailService;
            _teamService = teamService;
            _userService = userService;
        }

        [Route(StockRouter.GetAllStocks), HttpGet]
        public async Task<ActionResult<List<StockResponse>>> List()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [Route(StockRouter.GetStockById), HttpGet("{id}")]
        public async Task<ActionResult<StockResponse>> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Stock with id {id} not found" });
            return Ok(result);
        }

        [Route(StockRouter.AddStock), HttpPost]
        public async Task<ActionResult<StockResponse>> Add([FromBody] CreateStockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Auto-fill createdBy from JWT token
                request.CreatedBy = _userService.GetUsername(User);

                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.InnerException?.Message
                            ?? ex.InnerException?.Message
                            ?? ex.Message;
                return BadRequest(new { message = inner });
            }
        }

        [Route(StockRouter.UpdateStock), HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateStockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(new { message = "Cập nhật thành công", data = result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.InnerException?.Message
                            ?? ex.InnerException?.Message
                            ?? ex.Message;
                return BadRequest(new { message = inner });
            }
        }

        [Route(StockRouter.DeleteStock), HttpDelete("{id}")]
        [Authorize(Roles = "Quản lý kho")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Stock with id {id} not found" });
            return Ok(new { message = "Xóa thành công" });
        }

        [HttpGet("Warehouse/{warehouseId}")]
        public async Task<ActionResult<List<StockResponse>>> GetByWarehouse(int warehouseId)
        {
            var result = await _service.GetByWarehouseIdAsync(warehouseId);
            return Ok(result);
        }

        // Stock Detail endpoints
        [HttpPost("{stockId}/details")]
        public async Task<ActionResult<StockDetailResponse>> AddDetail(int stockId, [FromBody] CreateStockDetailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.StockCheckId = stockId;
                var result = await _detailService.CreateAsync(request);
                return Ok(new { message = "Thêm chi tiết kiểm kho thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{stockId}/details/{detailId}")]
        public async Task<ActionResult> UpdateDetail(int stockId, int detailId, [FromBody] CreateStockDetailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.StockCheckId = stockId;
                var result = await _detailService.UpdateAsync(detailId, request);
                return Ok(new { message = "Cập nhật thành công", data = result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{stockId}/details/{detailId}")]
        public async Task<ActionResult> DeleteDetail(int stockId, int detailId)
        {
            var result = await _detailService.DeleteAsync(detailId);
            if (!result)
                return NotFound(new { message = $"StockDetail with id {detailId} not found" });
            return Ok(new { message = "Xóa thành công" });
        }

        // Stock Team endpoints
        [HttpGet("teams")]
        public async Task<ActionResult<List<StockTeamResponse>>> GetAllTeams()
        {
            var result = await _teamService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{stockId}/teams")]
        public async Task<ActionResult<List<StockTeamResponse>>> GetTeams(int stockId)
        {
            var result = await _teamService.GetByStockCheckIdAsync(stockId);
            return Ok(result);
        }

        [HttpGet("{stockId}/teams/{teamId}")]
        public async Task<ActionResult<StockTeamResponse>> GetTeam(int stockId, int teamId)
        {
            var result = await _teamService.GetByIdAsync(teamId);
            if (result == null)
                return NotFound(new { message = $"StockTeam with id {teamId} not found" });
            return Ok(result);
        }

        [HttpPost("{stockId}/teams")]
        public async Task<ActionResult<StockTeamResponse>> AddTeam(int stockId, [FromBody] CreateStockTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.StockCheckId = stockId;
                var result = await _teamService.CreateAsync(request);
                return Ok(new { message = "Thêm đội kiểm kho thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{stockId}/teams/{teamId}")]
        public async Task<ActionResult> UpdateTeam(int stockId, int teamId, [FromBody] CreateStockTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.StockCheckId = stockId;
                var result = await _teamService.UpdateAsync(teamId, request);
                return Ok(new { message = "Cập nhật thành công", data = result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{stockId}/teams/{teamId}")]
        public async Task<ActionResult> DeleteTeam(int stockId, int teamId)
        {
            var result = await _teamService.DeleteAsync(teamId);
            if (!result)
                return NotFound(new { message = $"StockTeam with id {teamId} not found" });
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
