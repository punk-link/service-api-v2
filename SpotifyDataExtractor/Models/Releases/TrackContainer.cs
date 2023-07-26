using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct TrackContainer
{
    [JsonConstructor]
    public TrackContainer(List<Track> items)
    {
        Items = items;
    }


    [JsonPropertyName("items")]
    public List<Track> Items { get; init; }
}
