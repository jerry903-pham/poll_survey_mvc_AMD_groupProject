using AMD_Course_Work.Data;
using AMD_Course_Work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMD_Course_Work.Controllers
{
    [ApiController]
    [Route("polls")]
    public class PollController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PollController(AppDbContext context)
        {
            _context = context;
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

            var code = await GenerateUniqueCodeAsync();

            var poll = new Poll
            {
                Code = code,
                Question = request.Question.Trim(),
                IsClosed = false,
                CreatedAt = DateTime.UtcNow,
                Options = request.Options.Select(optionText => new Option
                {
                    Text = optionText.Trim(),
                    VoteCount = 0
                }).ToList()
            };

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            var response = new CreatePollResponse
            {
                Code = code,
                Link = $"/poll/{code}"
            };

            return CreatedAtAction(nameof(GetPoll), new { code }, response);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Poll>> GetPoll(string code)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound();

            return Ok(poll);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            while (true)
            {
                var code = new string(Enumerable.Range(0, 6)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());

                var exists = await _context.Polls.AnyAsync(p => p.Code == code);
                if (!exists)
                    return code;
            }
        }
    }
}