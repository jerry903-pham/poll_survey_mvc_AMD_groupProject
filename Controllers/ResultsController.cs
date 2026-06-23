using AMD_Course_Work.Dtos;
using AMD_Course_Work.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMD_Course_Work.Controllers
{
    [ApiController]
    [Route("polls/{code}")]
    public class ResultsController : ControllerBase
    {
        private readonly IPollService _pollService;

        public ResultsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet("results")]
        public async Task<ActionResult<PollResultsResponse>> GetResults(string code)
        {
            var results = await _pollService.GetResultsAsync(code);

            if (results == null)
                return NotFound("Poll not found.");

            return Ok(results);
        }
    }
}