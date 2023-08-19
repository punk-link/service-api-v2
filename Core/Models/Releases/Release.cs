using Core.Models.Releases.Enums;

namespace Core.Models.Releases;

public record Release
{
    public int Id { get; set; }
    public List<ImageDetails> ImageDetails { get; init; } = default!;
    public string Name { get; set; } = default!;
    public DateOnly ReleaseDate { get; set; }
    public ReleaseType Type { get; set; } = default!;
}
