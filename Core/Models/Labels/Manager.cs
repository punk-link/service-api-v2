namespace Core.Models.Labels;

public readonly record struct Manager
{
    public Manager()
    { }


    public int Id { get; init; }
    public int LabelId {get; init; }
    public string Name { get; init; } = default!;
}
