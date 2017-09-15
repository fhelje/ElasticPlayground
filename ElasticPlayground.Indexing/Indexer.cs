using System.Collections.Generic;
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
                      .VersionType(VersionType.External)
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

        public async Task<IEnumerable<IndexResult>> IndexMany<T>(IEnumerable<SearchableItem<T>> items)
        {
            var retval = new List<IndexResult>();
           foreach (var itemBatch in items.Batch(_configuration.BatchSize))
           {
               var request = new BulkDescriptor();
               request.CreateMany(itemBatch).Index(_configuration.Index);
               var result = await _client.BulkAsync(request);
               if (result.ApiCall.HttpStatusCode.HasValue && result.ApiCall.HttpStatusCode.Value >= 300)
               {
                    
                   Log.Error(result.OriginalException, "Unable to bulk insert to elastic");
                   return new IndexResult[0];
               }
                foreach (var item in result.Items)
                {
                    IndexResult indexResult = CreateErrorResult(item);
                    LogError(indexResult);
                    retval.Add(indexResult);
                }
            }
           return retval;
        }

        private static void LogError(IndexResult indexResult)
        {
            if (indexResult.ErrorType == ErrorType.Failed)
                Log.Error(indexResult.Message);
            else if (indexResult.ErrorType == ErrorType.VersionConflict)
            {
                Log.Debug(indexResult.Message);
            }
        }

        private IndexResult CreateErrorResult(BulkResponseItemBase item)
        {
            string message = null;
            var errorType = ErrorType.None;
            if (!item.IsValid)
            {
                errorType = GetErrorType(item.Error);
                message = item.Error.Reason;
            }
            var indexResult = new IndexResult(item.IsValid, errorType, message);
            return indexResult;
        }

        private ErrorType GetErrorType(BulkError error)
        {
            if (error == null)
            {
                return ErrorType.None;
            }
            if (error.Type == "version_conflict_engine_exception")
            {
                return ErrorType.VersionConflict;
            }
            return ErrorType.Failed;
        }
    }
}
