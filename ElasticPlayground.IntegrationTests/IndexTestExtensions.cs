namespace ElasticPlayground.IntegrationTests
{
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
}
