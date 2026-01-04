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
            if (string.IsNullOrEmpty(request.RawMessage)) return BadRequest(new
            {
                error = "El mensaje no puede estar vacio."
            });

            try
            {
                var result = await geminiService.ProcessedWorkout(request);
                return Ok(new
                {
                    response = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "Ocurrió un error al procesar el mensaje.",
                    details = ex.Message
                });
            }
        }

    }
}
