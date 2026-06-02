using Microsoft.AspNetCore.Mvc;
using ShiftScheduler.Models;
using ShiftScheduler.Services;

namespace ShiftScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleController(IEmployeePredictionService employeepredictionService, IShiftPredictionService shiftPredictionService, IScheduleSuggestionService scheduleSuggestionService) : ControllerBase
    {
        private readonly IEmployeePredictionService _employeepredictionService = employeepredictionService;
        private readonly IShiftPredictionService _shiftPredictionService = shiftPredictionService;
        private readonly IScheduleSuggestionService _scheduleSuggestionService = scheduleSuggestionService;

        [HttpPost]
        public IActionResult ScoreCandidates([FromBody] ScoreCandidatesRequest request)
        {
            var result = _employeepredictionService.ScoreCandidates(request);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PredictShifts([FromBody] ShiftSuggestionRequest request)
        {
            var result = _shiftPredictionService.PredictShifts(request);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult GenerateSchedulePerPosition([FromBody] ScheduleSuggestionRequest request)
        {
            var result = _scheduleSuggestionService.GenerateSchedule(request);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult GeneratePdf([FromBody] DateTime date)
        {
            var pdf = _scheduleSuggestionService.Generate(date);

            return File(
                pdf,
                "application/pdf",
                $"schedule_{date:yyyyMMdd}.pdf");
        }
    }
}