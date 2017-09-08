using System;

namespace ElasticPlayground
{
    // TODO: How to handle Id and version?
    public class SearchableItem<T>
    {
        public T Data { get; set; }
        public string Text { get; set; }
        public string BoostedText { get; set; }
        public Attributes<int>[] IntFilters { get; set; }
        public Attributes<string>[] StringFilters { get; set; }
        public Attributes<DateTime>[] DateFilters { get; set; }
        public Attributes<bool>[] BooleanFilters { get; set; }
        public Attributes<decimal>[] DecimalFilters{ get; set; }

        // TODO: Specialized
        // TODO: Text search: Should we have language? Stemming? On both Text and Boosted Text?
        // TODO: Autocomplete
        // TODO: Term suggester for "Did you  mean"
        // TODO: Special ranking calculations: Scores: { "bla": 0.3 }
    }
}