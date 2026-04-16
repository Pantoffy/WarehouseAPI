using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.DTOs.UnitDTOs;
using WarehouseAPI.Services.Unit;

namespace WarehouseAPI.Controllers.Unit
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController(IUnitService service) : ControllerBase
    {
        [Route(UnitRouter.GetAllUnits), HttpGet]
        public async Task<ActionResult<List<UnitResponse>>> GetAllUnits()
            => Ok(await service.GetAllUnitsAsync());

        [Route(UnitRouter.GetUnitById), HttpGet("{id}")]
        public async Task<ActionResult<UnitResponse?>> GetUnitById(int id)
        {
            var unit = await service.GetUnitByIdAsync(id);
            return unit is null ? NotFound("Không tìm thấy đơn vị với Id đã cho.") : Ok(unit);
        }
    }
}
