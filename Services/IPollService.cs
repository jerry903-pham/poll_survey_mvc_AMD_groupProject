using AMD_Course_Work.Dtos;

namespace AMD_Course_Work.Services
{
    public interface IPollService
    {
        Task<CreatePollResponse> CreatePollAsync(CreatePollRequest request);
        Task<PollDetailsResponse?> GetPollAsync(string code);
        Task<OperationResult> VoteAsync(string code, VoteRequest request);
        Task<PollResultsResponse?> GetResultsAsync(string code);
        Task<OperationResult> ClosePollAsync(string code);
    }
}