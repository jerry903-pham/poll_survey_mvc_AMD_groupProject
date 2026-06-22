namespace AMD_Course_Work.Models
{
    public class CreatePollRequest
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }
}