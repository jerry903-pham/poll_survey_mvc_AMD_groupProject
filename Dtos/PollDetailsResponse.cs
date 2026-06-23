namespace AMD_Course_Work.Dtos
{
    public class PollDetailsResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PollOptionResponse> Options { get; set; } = new();
    }

    public class PollOptionResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}