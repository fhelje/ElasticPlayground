using System;
using System.Threading.Tasks;
using ElasticPlayground.Indexing;
using Nest;
using Xunit;
using Shouldly;
using Serilog;
using System.Text;

namespace ElasticPlayground.Tests
{

    public class VersionTests
    {
        public VersionTests()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.ColoredConsole()
                            .CreateLogger();

        }
        [Fact(Skip = "")]
        public async Task should_be_valid_when_indexing_first_version()
        {
            var config = new Configuration
            {
                Index = "should_be_valid_when_indexing_first_version",
                BatchSize = 1
            };
            var client = CreateIndex(config);

            var sut = new Indexer(client, config);
            var item = new IndexTest
            {
                Id = 1,
                Version = 1,
                Name = "Name",
                Active = true
            }.ToSearchableItem();
            var indexResult = await sut.Index(item);
            indexResult.ShouldBeTrue();

            CleanUpIndex(client, config);
        }

        [Fact(Skip = "")]
        public async Task should_returnfalse_when_indexing_first_same_version()
        {
            var config = new Configuration
            {
                Index = "should_returnfalse_when_indexing_first_same_version",
                BatchSize = 1
            };
            var client = CreateIndex(config);

            var sut = new Indexer(client, config);
            var item = new IndexTest
            {
                Id = 1,
                Version = 2,
                Name = "Name",
                Active = true
            }.ToSearchableItem();
            var indexResult = await sut.Index(item);
            item = new IndexTest
            {
                Id = 1,
                Version = 1,
                Name = "Name2",
                Active = false
            }.ToSearchableItem();
            indexResult = await sut.Index(item);
            indexResult.ShouldBeFalse();

            CleanUpIndex(client, config);
        }

        private static void CleanUpIndex(ElasticClient client, Configuration config)
        {
            client.DeleteIndex(config.Index);
        }

        private static ElasticClient CreateIndex(Configuration config)
        {
            var connection = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DisableDirectStreaming()
    .OnRequestCompleted(apiCallDetails =>
    {
        // log out the request and the request body, if one exists for the type of request
        if (apiCallDetails.RequestBodyInBytes != null)
        {
            Log.Information(
                $"{apiCallDetails.HttpMethod} {apiCallDetails.Uri} " +
                $"{Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes)}");
        }
        else
        {
            Log.Information($"{apiCallDetails.HttpMethod} {apiCallDetails.Uri}");
        }

        // log out the response and the response body, if one exists for the type of response
        if (apiCallDetails.ResponseBodyInBytes != null)
        {
            Log.Information($"Status: {apiCallDetails.HttpStatusCode}" +
                     $"{Encoding.UTF8.GetString(apiCallDetails.ResponseBodyInBytes)}");
        }
        else
        {
            Log.Information($"Status: {apiCallDetails.HttpStatusCode}");
        }
    });
            var client = new ElasticClient(connection);
            var indexMappings = new IndexMappings(config, client);
            var result = indexMappings.CreateIndex();
            result.IsValid.ShouldBeTrue();
            return client;
        }
    }
}
