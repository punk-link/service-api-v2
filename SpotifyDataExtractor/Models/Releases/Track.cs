using System.Text.Json.Serialization;
using SpotifyDataExtractor.Models.Artists;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct Track
{
    [JsonConstructor]
    public Track(string id, List<SlimArtist> artists, int discNumber, int durationMilliseconds, bool isExplicit, string name, int trackNumber)
    {
        Id = id;
        Artists = artists;
        DiscNumber = discNumber;
        DurationMilliseconds = durationMilliseconds;
        IsExplicit = isExplicit;
        Name = name;
        TrackNumber = trackNumber;
    }


    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("artists")]
    public List<SlimArtist> Artists { get; init; }
    [JsonPropertyName("disc_number")]
    public int DiscNumber { get; init; }
    [JsonPropertyName("duration_ms")]
    public int DurationMilliseconds { get; init; }
    [JsonPropertyName("explicit")]
    public bool IsExplicit { get; init; }
    [JsonPropertyName("name")]
    public string Name { get; init; }
    [JsonPropertyName("track_number")]
    public int TrackNumber { get; init; }
}
