using System;
using Nest;

namespace ElasticPlayground.Indexing
{
    public class IndexMappings
    {
        private const string LowercaseKeywordAnalyserName = "lowercase-keyword";
        private readonly IConfiguration _config;
        private readonly IElasticClient _client;
        private static string SwedishTextAnalyserName = "swedish-text-analyser";

        public IndexMappings(IConfiguration config, IElasticClient client)
        {
            _config = config;
            _client = client;
        }

        public ICreateIndexResponse CreateIndex()
        {
            return _client.CreateIndex(_config.Index.PascalToKebabCase(), i => i
                .Settings(s => s
                    .NumberOfShards(2)
                    .NumberOfReplicas(0)
                    .Analysis(Analysis)
                )
                .Mappings(m => m
                    .Map<SearchableItem<object>>(SearchableItemMapper)
                )
            );
        }

        private static TypeMappingDescriptor<SearchableItem<T>> SearchableItemMapper<T>(TypeMappingDescriptor<SearchableItem<T>> map) where T : class => map
            .Properties(ps => ps
                .Object<T>(x=>x.Name(p=>p.Data).Enabled(false).IncludeInAll(false))
                .Text(t => t.Name(p => p.Text))
                .Text(t=>t.Name(p=>p.BoostedText).Boost(2))
                .Nested<Attributes<int>>(n => n
                    .Name(p => p.IntFilters)
                    .Properties(pps => pps
                        .Keyword(x => x.Name(p => p.Name))
                        .Number(x => x.Name(p => p.Value))
                    )
                )
                .Nested<Attributes<string>>(n => n
                    .Name(p => p.StringFilters)
                    .Properties(pps => pps
                        .Keyword(x => x.Name(p => p.Name))
                        .Keyword(x => 
                            x.Name(p => p.Value)
                                .Fields(f=>
                                    f.Text(t=>t.Name("lowercase").Analyzer(LowercaseKeywordAnalyserName))))
                    )
                )
                .Nested<Attributes<DateTime>>(n => n
                    .Name(p => p.DateFilters)
                    .Properties(pps => pps
                        .Keyword(x => x.Name(p => p.Name))
                        .Keyword(x => x.Name(p => p.Value))
                    )
                )
                .Nested<Attributes<bool>>(n => n
                    .Name(p => p.BooleanFilters)
                    .Properties(pps => pps
                        .Keyword(x => x.Name(p => p.Name))
                        .Boolean(x => x.Name(p => p.Value))
                    )
                )
                .Nested<Attributes<decimal>>(n => n
                    .Name(p => p.DecimalFilters)
                    .Properties(pps => pps
                        .Keyword(x => x.Name(p => p.Name))
                        .Number(x => x.Name(p => p.Value))
                    )
                )
            );
        private static AnalysisDescriptor Analysis(AnalysisDescriptor analysis) => analysis
            .CharFilters(c=>c.Mapping("swedish_char_mapping", m=>m.Mappings("w => v", "W => V")))
            .TokenFilters(tf=>tf.Hunspell("sv_SE", x=>x.Dedup(true).Locale("sv_SE")))
            .Analyzers(analyzers => analyzers
                .Custom(LowercaseKeywordAnalyserName, c => c
                    .Tokenizer("keyword")
                    .Filters("lowercase")
                )
                .Custom(SwedishTextAnalyserName, c => c
                    .Tokenizer("standard")
                    .Filters("lowercase", "sv_SE")
                    .CharFilters("html_strip", "swedish_char_mapping")
                    
                )
            );
    }
}