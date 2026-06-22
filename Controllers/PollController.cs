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

        [HttpPost("{code}/vote")]
        public async Task<IActionResult> Vote(string code, [FromBody] VoteRequest request)
        {
            if (request.OptionId <= 0)
                return BadRequest("OptionId is required.");

            if (string.IsNullOrWhiteSpace(request.VoterToken))
                return BadRequest("VoterToken is required.");

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound("Poll not found.");

            if (poll.IsClosed)
                return BadRequest("Poll is closed.");

            var option = poll.Options.FirstOrDefault(o => o.Id == request.OptionId);

            if (option == null)
                return BadRequest("Invalid option for this poll.");

            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v =>
                    v.PollId == poll.Id &&
                    v.VoterToken == request.VoterToken);

            if (existingVote != null)
                return Conflict("You have already voted in this poll.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var vote = new Vote
                {
                    PollId = poll.Id,
                    OptionId = request.OptionId,
                    VoterToken = request.VoterToken
                };

                option.VoteCount += 1;

                _context.Votes.Add(vote);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Vote recorded successfully."
                });
            }
            catch
            {
                await transaction.RollbackAsync();

                return StatusCode(500,
                    "An error occurred while saving the vote.");
            }
        }

        [HttpGet("{code}/results")]
        public async Task<ActionResult<PollResultsResponse>> GetResults(string code)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return NotFound("Poll not found.");

            var response = new PollResultsResponse
            {
                Question = poll.Question,
                Results = poll.Options.Select(o => new ResultOption
                {
                    Option = o.Text,
                    Votes = o.VoteCount
                }).ToList()
            };

            return Ok(response);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            while (true)
            {
                var code = new string(
                    Enumerable.Range(0, 6)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());

                var exists = await _context.Polls
                    .AnyAsync(p => p.Code == code);

                if (!exists)
                    return code;
            }
        }
    }
}