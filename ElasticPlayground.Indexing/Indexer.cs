using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Serilog;

namespace ElasticPlayground.Indexing
{
    public class Indexer
    {
        private readonly IElasticClient _client;
        private readonly IConfiguration _configuration;

        public Indexer(IElasticClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<bool> Index<T>(SearchableItem<T> item)
        {
            var result = await _client.IndexAsync(
                item, 
                x => x.Id(new Id(item.Id))
                      .Version(item.Version)
                      .VersionType(VersionType.ExternalGte)
                      .Index(_configuration.Index)
            );

            if (result.IsValid)
            {
                return true;
            }
            // Check if version error and log as info
            // LOG
            Log.Error(result.OriginalException, "Error indexing to elastic. Item id: {id}, version {version}", item.Id, item.Version);
            return false;
        }

        //public async Task<IEnumerable<IndexResult>> IndexMany<T>(IEnumerable<SearchableItem<T>> items)
        //{
        //    foreach (var itemBatch in items.Batch(_configuration.BatchSize))
        //    {
        //        IBulkRequest request = new BulkDescriptor();
        //        var result = await _client.BulkAsync(request);
        //        if (!result.IsValid)
        //        {
                    
        //            Log.Error(result.OriginalException, "Unable to bulk insert to elastic");
        //            return new IndexResult[0];
        //        }
        //        if (result.Errors)
        //        {
        //            foreach (var item in result.ItemsWithErrors)
        //            {
        //                if (item.Error.Reason.Contains("asdf"))
        //                {
                            
        //                }
        //            }
        //        }
        //    }
        //    return await Task.FromResult(new List<IndexResult>());
        //    //
            
        //}
              
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }

    public class IndexResult
    {
    }
}
