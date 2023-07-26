using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Artists;

public readonly record struct SlimArtist
{
    [JsonConstructor]
    public SlimArtist(string id, /*List<ImageDetails> imageDetails,*/ string name)
    {
        Id = id;
        //ImageDetails = imageDetails;
        Name = name;
    }


    [JsonPropertyName("id")]
    public string Id { get; init; }
    //[JsonPropertyName("images")]
    //public List<ImageDetails> ImageDetails { get; init; }
    [JsonPropertyName("name")]
    public string Name { get; init; }
}
