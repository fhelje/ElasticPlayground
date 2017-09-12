namespace ElasticPlayground.Indexing
{
    public class IndexResult
    {
        private bool _isValid;
        private ErrorType _errorType;
        private string _reason;

        public IndexResult(bool isValid, ErrorType errorType, string reason)
        {
            _isValid = isValid;
            _errorType = errorType;
            _reason = reason;
        }

        public bool IsValid => _isValid;

        public ErrorType ErrorType => _errorType;
        public string Message => _reason;
    }
}
