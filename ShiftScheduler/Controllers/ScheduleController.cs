using Microsoft.AspNetCore.Mvc;
using ShiftScheduler.Services;

namespace ShiftScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly PredictionService _predictionService;

        public ScheduleController()
        {
            _predictionService = new PredictionService("ML/model.zip");
        }

        [HttpGet("scoreCandidates")]
        public IActionResult ScoreCandidates(
            [FromQuery] string positionId,
            [FromQuery] int shiftType,
            [FromQuery] int dayOfWeek,
            [FromQuery] int month,
            [FromQuery] bool isWeekend,
            [FromQuery] DateTime date)
        {
            var result = _predictionService.ScoreCandidates(
                positionId,
                shiftType,
                dayOfWeek,
                month,
                isWeekend,
                date);

            return Ok(result);
        }
    }
}