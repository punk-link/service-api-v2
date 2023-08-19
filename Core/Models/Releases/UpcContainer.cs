namespace Core.Models.Releases;

public readonly record struct UpcContainer
{
    public UpcContainer()
    {
    }

    public int Id { get; init; }
    public string Upc { get; init; } = default!;
}
