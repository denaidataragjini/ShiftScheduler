using Microsoft.AspNetCore.Mvc;
using ShiftScheduler.Models;
using ShiftScheduler.Services;

namespace ShiftScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleController(IPredictionService predictionService, IShiftPredictionService shiftPredictionService) : ControllerBase
    {
        private readonly IPredictionService _predictionService = predictionService;
        private readonly IShiftPredictionService _shiftPredictionService = shiftPredictionService;

        [HttpPost]
        public IActionResult ScoreCandidates([FromBody] ScoreCandidatesRequest request)
        {
            var result = _predictionService.ScoreCandidates(request);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PredictShifts([FromBody] ShiftSuggestionRequest request)
        {
            var result = _shiftPredictionService.PredictShifts(request);

            return Ok(result);
        }
    }
}