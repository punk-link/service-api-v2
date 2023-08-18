namespace Core.Models.Artists;

public readonly record struct ArtistSearchResult
{
    public ArtistSearchResult()
    {
    }


    public string SpotifyId { get; init; } = default!;
    public List<ImageDetails> ImageDetails { get; init; } = default!;
    public string Name { get; init; } = default!;
}
