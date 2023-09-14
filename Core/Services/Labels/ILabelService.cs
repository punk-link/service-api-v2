using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Labels;

public interface ILabelService
{
    public Task<Result<Label>> Add(string labelName, CancellationToken cancellationToken = default);
    public Task<Label> Get(ManagerContext currentManager, int id, CancellationToken cancellationToken = default);
    public Task<Result<Label>> Modify(ManagerContext currentManager, Label label, CancellationToken cancellationToken = default);
}
