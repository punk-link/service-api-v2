namespace Core.Models.Artists;

public readonly record struct ArtistIdContainer
{
    public int Id { get; init; }
    public string SpotifyId { get; init; }
}
