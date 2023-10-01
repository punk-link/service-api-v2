namespace Core.Models.Labels;

public readonly record struct Label
{
    public Label()
    { }


    public int Id { get; init; }
    public string Name { get; init; } = default!;
}
