using Core.Data.Labels;
using System.Linq.Expressions;

namespace Core.Converters.Labels;

public static class DbManagerConverter
{
    public static Manager ToDbManager(this Models.Labels.Manager manager, DateTime timeStamp)
        => new()
        {
            Created = timeStamp,
            LabelId = manager.LabelId,
            Name = manager.Name,
            Updated = timeStamp
        };


    public static Models.Labels.Manager ToManager(this Manager dbManager) 
        => new()
        {
            Id = dbManager.Id,
            LabelId = dbManager.LabelId,
            Name = dbManager.Name
        };


    public static Models.Labels.ManagerContext ToManagerContext(this Models.Labels.Manager manager)
        => new()
        {
            Id = manager.Id,
            LabelId = manager.LabelId
        };
}
