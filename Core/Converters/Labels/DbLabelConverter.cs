using Core.Data.Labels;

namespace Core.Converters.Labels;

internal static class DbLabelConverter
{
    public static Label ToDbLabel(this Models.Labels.Label label, DateTime timeStamp)
        => new()
        {
            Created = timeStamp,
            Managers = new List<Manager>(0),
            Name = label.Name,
            Updated = timeStamp,
        };


    public static Models.Labels.Label ToLabel(this Label dbLabel) 
        => new()
        {
            Id = dbLabel.Id,
            Name = dbLabel.Name
        };
}
