using System;
using System.Threading.Tasks;
using ElasticPlayground.Indexing;
using Nest;
using Xunit;
using Shouldly;

namespace ElasticPlayground.Tests
{
    public class VersionTests
    {

        [Fact]
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

        [Fact]
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
                Version = 1,
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
            var client = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")));
            var indexMappings = new IndexMappings(config, client);
            var result = indexMappings.CreateIndex();
            result.IsValid.ShouldBeTrue();
            return client;
        }
    }

    public static class IndexTestExtensions
    {
        public static SearchableItem<IndexTest> ToSearchableItem(this IndexTest item)
        {
            return new SearchableItem<IndexTest>
            {
                Id = item.Id,
                Version = item.Version,
                Text = item.Name,
                BooleanFilters = new []
                {
                    new Attributes<bool>(nameof(item.Active), item.Active), 
                }
            };
        }
    }

    public class IndexTest
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int StockCount { get; set; }
        public string Departments { get; set; }
        public DateTime Published { get; set; }
    }

    public class MappingTests
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
