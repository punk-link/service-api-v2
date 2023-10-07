using System.Text.Json.Serialization;
using SpotifyDataExtractor.Models.Artists;
using SpotifyDataExtractor.Models.Common;
using SpotifyDataExtractor.Models.Enums;

namespace SpotifyDataExtractor.Models.Releases;

public readonly record struct Release
{
    [JsonConstructor]
    public Release(string id, List<SlimArtist> artists, ExternalIdContainer externalIds, ExternalUrlContainer externalUrls, List<string> genres,
        List<ImageDetails> imageDetails, string label, string name, string releaseDate, int trackNumber, TrackContainer tracks, ReleaseType type)
    {
        Id = id;
        Artists = artists;
        ExternalIds = externalIds;
        ExternalUrls = externalUrls;
        Genres = genres;
        ImageDetails = imageDetails;
        Label = label;
        Name = name;
        ReleaseDate = releaseDate;
        TrackNumber = trackNumber;
        Tracks = tracks;
        Type = type;
    }


    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("artists")]
    public List<SlimArtist> Artists { get; init; }

    [JsonPropertyName("external_ids")]
    public ExternalIdContainer ExternalIds { get; init; }

    [JsonPropertyName("external_urls")]
    public ExternalUrlContainer ExternalUrls { get; init; }

    [JsonPropertyName("genres")]
    public List<string> Genres { get; init; }

    [JsonPropertyName("images")]
    public List<ImageDetails> ImageDetails { get; init; }

    [JsonPropertyName("label")]
    public string Label { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; init; }

    [JsonPropertyName("total_tracks")]
    public int TrackNumber { get; init; }

    [JsonPropertyName("tracks")]
    public TrackContainer Tracks { get; init; }

    [JsonPropertyName("album_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReleaseType Type { get; init; }
}
