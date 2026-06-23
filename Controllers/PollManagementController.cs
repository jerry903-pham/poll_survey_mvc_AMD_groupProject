using AMD_Course_Work.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMD_Course_Work.Controllers
{
    [ApiController]
    [Route("polls/{code}")]
    public class PollManagementController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollManagementController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost("close")]
        public async Task<IActionResult> ClosePoll(string code)
        {
            var result = await _pollService.ClosePollAsync(code);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}