using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticPlayground.Indexing;
using Nest;
using Serilog;
using Shouldly;
using Xunit;

namespace ElasticPlayground.IntegrationTests
{
    public class BulkIndexTests
    {
        public BulkIndexTests()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.ColoredConsole()
                            .CreateLogger();
        }

        [Fact]
        public async Task should_load_a_lot_of_stuff()
        {
            var config = new Configuration
            {
                Index = "should_load_a_lot_of_stuff",
                BatchSize = 3
            };

            var client = CreateIndex(config);
            var data = IndexTestBuilder.With().Create(10).Build();
            var sut = new Indexer(client, config);
            var result = await sut.IndexMany(data.Select(x=>x.ToSearchableItem()));
            
            result.Any(x=>x.IsValid == false).ShouldBeFalse();

            CleanUpIndex(client, config);
        }

        [Fact]
        public async Task should_return_error_if_same_id_exists()
        {
            var config = new Configuration
            {
                Index = "should_return_error_if_same_id_exists",
                BatchSize = 3
            };

            var client = CreateIndex(config);
            var data = IndexTestBuilder.With().CreateWithSameId(10).Build();
            var sut = new Indexer(client, config);
            var result = await sut.IndexMany(data.Select(x=>x.ToSearchableItem()));

            var indexResults = result as IList<IndexResult> ?? result.ToList();
            indexResults.Count(x => x.IsValid).ShouldBe(5);
            indexResults.Count(x=>x.IsValid == false).ShouldBe(5);

            CleanUpIndex(client, config);
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

        private static void CleanUpIndex(ElasticClient client, Configuration config)
        {
            client.DeleteIndex(config.Index);
        }
    }
}
