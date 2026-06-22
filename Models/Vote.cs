namespace AMD_Course_Work.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int OptionId { get; set; }
        public string VoterToken { get; set; } = string.Empty;
    }
}