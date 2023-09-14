namespace Core.Models.Labels;

public readonly record struct MasterManagerRequest
{
    public MasterManagerRequest()
    { }


    public string LabelName { get; init; } = default!;
    public string ManagerName { get; init; } = default!;
}
