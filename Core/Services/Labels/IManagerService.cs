using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Labels;

public interface IManagerService
{
    public Task<Result<Manager>> Add(ManagerContext currentManager, Manager manager, CancellationToken cancellationToken = default);
    public Task<Result<Manager>> AddMaster(string labelName, string managerName, CancellationToken cancellationToken = default);
    public Task<List<Manager>> Get(ManagerContext currentManager, CancellationToken cancellationToken = default);
    public Task<Manager> Get(ManagerContext currentManager, int managerId, CancellationToken cancellationToken = default);
    public Task<ManagerContext> GetContext(int managerId, CancellationToken cancellationToken = default);
    public Task<Result<Manager>> Modify(ManagerContext currentManager, Manager manager);
}
