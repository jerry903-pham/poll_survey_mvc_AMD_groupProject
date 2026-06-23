using AMD_Course_Work.Dtos;
using AMD_Course_Work.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMD_Course_Work.Controllers
{
    [ApiController]
    [Route("polls/{code}")]
    public class VoteController : ControllerBase
    {
        private readonly IPollService _pollService;

        public VoteController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost("vote")]
        public async Task<IActionResult> Vote(string code, [FromBody] VoteRequest request)
        {
            var result = await _pollService.VoteAsync(code, request);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}