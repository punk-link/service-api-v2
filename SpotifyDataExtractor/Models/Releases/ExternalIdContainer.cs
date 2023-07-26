using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct ExternalIdContainer
{
    [JsonConstructor]
    public ExternalIdContainer(string upc)
    {
        Upc = upc;
    }


    [JsonPropertyName("upc")]
    public string Upc { get; init; }
}
