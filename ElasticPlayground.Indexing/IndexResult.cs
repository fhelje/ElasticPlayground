namespace ElasticPlayground.Indexing
{
    public class IndexResult
    {
        public IndexResult(bool isValid, ErrorType errorType, string reason)
        {
            IsValid = isValid;
            ErrorType = errorType;
            Message = reason;
        }

        public bool IsValid { get; }

        public ErrorType ErrorType { get; }

        public string Message { get; }
    }
}
