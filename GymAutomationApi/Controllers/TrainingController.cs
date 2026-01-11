using GymAutomationApi.Interfaces;
using GymAutomationApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymAutomationApi.Controllers
{
    [Route("api/training")]
    [ApiController]
    public class TrainingController : ControllerBase
    {

        private readonly IGeminiService geminiService;

        public TrainingController(IGeminiService geminiService)
        {
            this.geminiService = geminiService;
        }

        [HttpPost("process-workout")]
        public async Task<IActionResult> ProcessWorkout([FromBody] WorkoutRequest request)
        {
            var result = await geminiService.ProcessedWorkout(request);
            return Ok(result);
        }

    }
}
