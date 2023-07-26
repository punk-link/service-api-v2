namespace Core.Models.Labels;

public readonly record struct ManagerContext
{
    public ManagerContext()
    {
    }

    public int Id { get; init; } = default!;
    public int LabelId { get; init; } = default!;
}
