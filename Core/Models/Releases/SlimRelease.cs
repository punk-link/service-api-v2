namespace Core.Models.Releases;

public record SlimRelease
{
    public int Id { get; set; }
    public ImageDetails ImageDetails { get; init; } = default!;
    public string Name { get; set; } = default!;
    public DateOnly ReleaseDate { get; set; }
    public string Type { get; set; } = default!;
}
