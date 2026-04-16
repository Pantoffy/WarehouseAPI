using Microsoft.AspNetCore.Mvc;
using WarehouseAPI.Services.Calendar;

namespace WarehouseAPI.Controllers.Calendar
{
    [ApiController]
    [Route("api/calendar")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        /// <summary>
        /// Lấy tất cả event (có thể filter theo tháng/năm)
        /// GET /api/calendar/events
        /// GET /api/calendar/events?month=4&year=2026
        /// </summary>
        [HttpGet("events")]
        public async Task<IActionResult> GetEvents([FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                var events = await _calendarService.GetEventsAsync(month, year);
                return Ok(new
                {
                    success = true,
                    data = events,
                    count = events.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy event trong khoảng thời gian cụ thể
        /// GET /api/calendar/events/range?startDate=2026-04-01&endDate=2026-04-30
        /// </summary>
        [HttpGet("events/range")]
        public async Task<IActionResult> GetEventsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "startDate không thể lớn hơn endDate"
                    });
                }

                var events = await _calendarService.GetEventsByDateRangeAsync(startDate, endDate);
                return Ok(new
                {
                    success = true,
                    data = events,
                    count = events.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
