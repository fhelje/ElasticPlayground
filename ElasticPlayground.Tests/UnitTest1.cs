using System;
using ElasticPlayground.Indexing;
using Nest;
using Xunit;

namespace ElasticPlayground.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var config = new Configuration{Index = "Test1"};
            var client = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")));
            var indexMappings = new IndexMappings(config, client);
            var result = indexMappings.CreateIndex();
            if (!result.IsValid)
            {
                Console.WriteLine(result.ServerError.Error.Reason);
            }


        }
    }
}
