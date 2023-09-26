using Core.Models.Labels;

namespace CoreTests.Shared;

internal static class ManagerContexts
{
    public static readonly ManagerContext DefaultAddingManagerContext = new()
    {
        Id = 1,
        LabelId = 1
    };

    public static readonly ManagerContext DefaultModifyingManagerContext = new()
    {
        Id = 1,
        LabelId = 2
    };

    public static readonly ManagerContext DifferentLabelManagerContext = new()
    {
        Id = 1,
        LabelId = 3
    };
}