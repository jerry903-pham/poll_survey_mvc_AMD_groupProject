namespace AMD_Course_Work.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Option> Options { get; set; } = new();
    }
}