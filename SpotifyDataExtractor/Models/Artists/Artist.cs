using System.Text.Json.Serialization;
using SpotifyDataExtractor.Models.Common;

namespace SpotifyDataExtractor.Models.Artists;

public readonly record struct Artist
{
    [JsonConstructor]
    public Artist(string id, List<string> genres, List<ImageDetails> imageDetails, string name)
    {
        Id = id;
        Genres = genres;
        ImageDetails = imageDetails;
        Name = name;
    }


    [JsonPropertyName("id")]
    public string Id { get; init; }
    [JsonPropertyName("genres")]
    public List<string> Genres { get; init; }
    [JsonPropertyName("images")]
    public List<ImageDetails> ImageDetails { get; init; }
    [JsonPropertyName("name")]
    public string Name { get; init; }
}
