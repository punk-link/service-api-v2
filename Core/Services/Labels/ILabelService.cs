using Core.Models.Labels;

namespace Core.Services.Labels;

public interface ILabelService
{
    public Task<Label> Add(string labelName, CancellationToken cancellationToken = default);
}
