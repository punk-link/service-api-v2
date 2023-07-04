using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Common;

public readonly record struct ImageDetails
{
    [JsonConstructor]
    public ImageDetails(int height, string url, int width)
    {
        Height = height;
        Url = url;
        Width = width;
    }


    [JsonPropertyName("height")]
    public int Height { get; init; }
    [JsonPropertyName("url")]
    public string Url { get; init; }
    [JsonPropertyName("width")]
    public int Width { get; init; }
}
