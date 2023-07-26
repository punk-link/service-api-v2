using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct ReleaseContainer
{
    [JsonConstructor]
    public ReleaseContainer(List<Release> items, string next)
    {
        Items = items;
        Next = next;
    }


    [JsonPropertyName("items")]
    public List<Release> Items { get; init; }
    [JsonPropertyName("next")]
    public string Next { get; init; }
}
