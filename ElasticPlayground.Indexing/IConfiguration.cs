namespace ElasticPlayground.Indexing
{
    public interface IConfiguration
    {
        string Index { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public string Index { get; set; }
    }
}