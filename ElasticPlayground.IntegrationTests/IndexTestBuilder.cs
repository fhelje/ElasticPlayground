using System.Collections.Generic;
using System.Linq;

namespace ElasticPlayground.IntegrationTests
{
    public class IndexTestBuilder
    {
        private IEnumerable<IndexTest> _data;

        private IndexTestBuilder()
        {

        }
        public static IndexTestBuilder With()
        {
            return new IndexTestBuilder();
        }

        public IndexTestBuilder Create(int count)
        {
            _data = Enumerable.Range(0, count).Select(x=>new IndexTest{ Id = x, Version = 1, Name = $"Name {x}", Active = true}).ToList();
            return this;
        }

        public IEnumerable<IndexTest> Build()
        {
            return _data;
        }

        internal IndexTestBuilder CreateWithSameId(int count)
        {
            var half = count / 2;
            var temp = new List<IndexTest>();
            var id = 1;
            foreach (var index in Enumerable.Range(0,count))
            {
                temp.Add(new IndexTest
                {
                    Id = id,
                    Version = 1,
                    Name = $"Name {id}:{index}",
                    Active = index % 2 == 0
                });
                if (id >= half)
                {
                    id=0;
                }
                id++;
            }
            _data = temp;
            return this;
        }
    }
}
