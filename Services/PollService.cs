using AMD_Course_Work.Data;
using AMD_Course_Work.Dtos;
using AMD_Course_Work.Models;
using Microsoft.EntityFrameworkCore;

namespace AMD_Course_Work.Services
{
    public class PollService : IPollService
    {
        private readonly AppDbContext _context;

        public PollService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreatePollResponse> CreatePollAsync(CreatePollRequest request)
        {
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

            return new CreatePollResponse
            {
                Code = code,
                Link = $"/poll/{code}"
            };
        }

        public async Task<PollDetailsResponse?> GetPollAsync(string code)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return null;

            return new PollDetailsResponse
            {
                Id = poll.Id,
                Code = poll.Code,
                Question = poll.Question,
                IsClosed = poll.IsClosed,
                CreatedAt = poll.CreatedAt,
                Options = poll.Options.Select(o => new PollOptionResponse
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            };
        }

        public async Task<OperationResult> VoteAsync(string code, VoteRequest request)
        {
            if (request == null)
                return Fail("Request is required.", 400);

            if (request.OptionId <= 0)
                return Fail("OptionId is required.", 400);

            if (string.IsNullOrWhiteSpace(request.VoterToken))
                return Fail("VoterToken is required.", 400);

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return Fail("Poll not found.", 404);

            if (poll.IsClosed)
                return Fail("Poll is closed.", 400);

            var option = poll.Options.FirstOrDefault(o => o.Id == request.OptionId);

            if (option == null)
                return Fail("Invalid option for this poll.", 400);

            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v => v.PollId == poll.Id && v.VoterToken == request.VoterToken);

            if (existingVote != null)
                return Fail("You have already voted in this poll.", 409);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var vote = new Vote
                {
                    PollId = poll.Id,
                    OptionId = option.Id,
                    VoterToken = request.VoterToken
                };

                option.VoteCount += 1;

                _context.Votes.Add(vote);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Success("Vote recorded successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return Fail("An error occurred while saving the vote.", 500);
            }
        }

        public async Task<PollResultsResponse?> GetResultsAsync(string code)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return null;

            return new PollResultsResponse
            {
                Question = poll.Question,
                Results = poll.Options.Select(o => new ResultOption
                {
                    Option = o.Text,
                    Votes = o.VoteCount
                }).ToList()
            };
        }

        public async Task<OperationResult> ClosePollAsync(string code)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Code == code);

            if (poll == null)
                return Fail("Poll not found.", 404);

            if (poll.IsClosed)
                return Success("Poll is already closed.");

            poll.IsClosed = true;
            await _context.SaveChangesAsync();

            return Success("Poll closed successfully.");
        }

        private static OperationResult Success(string message)
        {
            return new OperationResult
            {
                Success = true,
                StatusCode = 200,
                Message = message
            };
        }

        private static OperationResult Fail(string message, int statusCode)
        {
            return new OperationResult
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
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

                var exists = await _context.Polls.AnyAsync(p => p.Code == code);

                if (!exists)
                    return code;
            }
        }
    }
}