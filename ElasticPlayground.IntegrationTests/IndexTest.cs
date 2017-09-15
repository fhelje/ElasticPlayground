using System;

namespace ElasticPlayground.IntegrationTests
{
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
}
