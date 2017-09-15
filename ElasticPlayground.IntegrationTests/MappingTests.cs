using System;
using ElasticPlayground.Indexing;
using Nest;
using Serilog;
using Shouldly;
using Xunit;

namespace ElasticPlayground.IntegrationTests
{

    public class MappingTests
    {
        public MappingTests()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.ColoredConsole()
                            .CreateLogger();

        }
        [Fact]
        public void Test1()
        {
            var config = new Configuration{Index = "Test1"};
            var client = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")));
            var indexMappings = new IndexMappings(config, client);
            var result = indexMappings.CreateIndex();

            result.IsValid.ShouldBeTrue();

            CleanUpIndex(client, config);
        }
        private static void CleanUpIndex(ElasticClient client, Configuration config)
        {
            client.DeleteIndex(config.Index);
        }
    }
}
