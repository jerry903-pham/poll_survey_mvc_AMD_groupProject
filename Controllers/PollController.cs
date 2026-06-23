using AMD_Course_Work.Dtos;
using AMD_Course_Work.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMD_Course_Work.Controllers
{
    [ApiController]
    [Route("polls")]
    public class PollController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpPost]
        public async Task<ActionResult<CreatePollResponse>> CreatePoll([FromBody] CreatePollRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            if (request.Options == null || request.Options.Count < 2 || request.Options.Count > 6)
                return BadRequest("Poll must have between 2 and 6 options.");

            if (request.Options.Any(o => string.IsNullOrWhiteSpace(o)))
                return BadRequest("Options cannot be empty.");

            var response = await _pollService.CreatePollAsync(request);
            return CreatedAtAction(nameof(GetPoll), new { code = response.Code }, response);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<PollDetailsResponse>> GetPoll(string code)
        {
            var poll = await _pollService.GetPollAsync(code);

            if (poll == null)
                return NotFound();

            return Ok(poll);
        }
    }
}