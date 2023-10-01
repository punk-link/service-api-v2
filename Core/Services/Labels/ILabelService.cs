using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Labels;

public interface ILabelService
{
    public ValueTask<Result<Label>> Add(string? labelName, CancellationToken cancellationToken = default);
    public Task<Maybe<Label>> Get(ManagerContext currentManager, int id, CancellationToken cancellationToken = default);
    public ValueTask<Result<Label>> Modify(ManagerContext currentManager, Label label, CancellationToken cancellationToken = default);
}
