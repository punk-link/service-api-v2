namespace Core.Models.Artists;

public record SocialNetwork
{
    public string Id { get; init; } = default!;
    public string Url { get; init; } = default!;
}
