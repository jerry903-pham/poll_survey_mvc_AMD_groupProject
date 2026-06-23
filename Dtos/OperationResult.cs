namespace AMD_Course_Work.Dtos
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}