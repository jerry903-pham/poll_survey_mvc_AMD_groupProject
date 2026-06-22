using System.Text.Json.Serialization;

namespace AMD_Course_Work.Models
{
    public class Option
    {
        public int Id { get; set; }

        public int PollId { get; set; }

        public string Text { get; set; } = string.Empty;

        public int VoteCount { get; set; }

        [JsonIgnore]
        public Poll Poll { get; set; } = null!;
    }
}