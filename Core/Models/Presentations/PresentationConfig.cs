namespace Core.Models.Presentations;

public record PresentationConfig
{
    public List<string> ShareableSocialNetworkIds { get; init; } = default!;
}
