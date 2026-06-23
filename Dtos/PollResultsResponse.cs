namespace AMD_Course_Work.Models
{
    public class PollResultsResponse
    {
        public string Question { get; set; } = string.Empty;

        public List<ResultOption> Results { get; set; } = new();
    }

    public class ResultOption
    {
        public string Option { get; set; } = string.Empty;

        public int Votes { get; set; }
    }
}