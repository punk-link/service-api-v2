using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct ExternalUrlContainer
{
    [JsonConstructor]
    public ExternalUrlContainer(string spotify)
    {
        Spotify = spotify;
    }


    [JsonPropertyName("spotify")]
    public string Spotify { get; init; }
}
