namespace ElasticPlayground.Indexing
{
    public interface IConfiguration
    {
        string Index { get; set; }
        int BatchSize { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public string Index { get; set; }
        public int BatchSize { get; set; }  
    }
}