namespace AMD_Course_Work.Dtos
{
    public class VoteRequest
    {
        public int OptionId { get; set; }
        public string VoterToken { get; set; } = string.Empty;
    }
}