namespace AMD_Course_Work.Models
{
    public class VoteRequest
    {
        public int OptionId { get; set; }
        public string VoterToken { get; set; } = string.Empty;
    }
}