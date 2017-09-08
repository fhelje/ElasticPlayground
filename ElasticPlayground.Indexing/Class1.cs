using System.Threading.Tasks;
using Nest;

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
                x => x.Index(_configuration.Index)
            );

            if (result.IsValid)
            {
                return true;
            }
            // LOG
            return false;
        }
              
    }

}
