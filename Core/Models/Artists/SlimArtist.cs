namespace Core.Models.Artists;

public record SlimArtist
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
}
