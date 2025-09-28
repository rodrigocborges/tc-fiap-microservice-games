using FIAPCloudGames.Domain.Enumerators;
using Newtonsoft.Json;

namespace FIAPCloudGames.Application.Responses;
public class CategoryMetric
{
    [JsonProperty("key")]
    public GameCategory CategoryId { get; set; }

    [JsonProperty("doc_count")]
    public int GameCount { get; set; }
}
public class CategoryMetricResponse
{
    [JsonProperty("games_by_category")]
    public AggregationResult AggregationResult { get; set; }
}

public class AggregationResult
{
    [JsonProperty("buckets")]
    public List<CategoryMetric> Buckets { get; set; }
}