using Core.Converters.Labels;
using Core.Data;
using Core.Models.Labels;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Services.Labels;

public class ManagerService : IManagerService
{
    public ManagerService(Context context, ILabelService labelService)
    {
        _context = context;
        _labelService = labelService;
    }


    public async Task<Result<Manager>> Add(ManagerContext currentManager, Manager manager, CancellationToken cancellationToken = default)
    {
        var updatedManager = manager with { LabelId = currentManager.LabelId, Name = manager.Name.Trim() };
        var now = DateTime.UtcNow;

        return await Result.Success()
            .Ensure(() => !string.IsNullOrWhiteSpace(updatedManager.Name), "Manager's name not provided.")
            .Bind(() => AddInternal(manager, now, currentManager, cancellationToken));
    }


    public Task<Result<Manager>> AddMaster(string labelName, string managerName, CancellationToken cancellationToken = default)
    {
        var manager = new Manager
        {
            Name = managerName.Trim()
        };
        throw new NotImplementedException();
    }


    public async Task<List<Manager>> Get(ManagerContext currentManager, CancellationToken cancellationToken = default)
        => await GetInternal(x => x.LabelId == currentManager.LabelId)
            .ToListAsync(cancellationToken);

    public async Task<Manager> Get(ManagerContext currentManager, int managerId, CancellationToken cancellationToken = default)
    {
        var manager = await GetInternal(managerId, cancellationToken);
        if (manager.LabelId == currentManager.LabelId)
            return manager;

        return default;
    }


    public async Task<ManagerContext> GetContext(int managerId, CancellationToken cancellationToken = default)
    {
        var manager = await GetInternal(managerId, cancellationToken);
        return manager.ToManagerContext();
    }


    public Task<Result<Manager>> Modify(ManagerContext currentManager, Manager manager)
    {
        throw new NotImplementedException();
    }


    private async Task<Result<Manager>> AddInternal(Manager manager, DateTime timeStamp, ManagerContext managerContext = default, CancellationToken cancellationToken = default)
    {
        var dbManager = manager.ToDbManager(timeStamp);
        await _context.Managers.AddAsync(dbManager, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(await GetInternal(dbManager.Id, cancellationToken));
    }


    private async Task<Manager> GetInternal(int id, CancellationToken cancellationToken = default)
        => await GetInternal(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    

    // TODO: check query evaluation for select
    private IQueryable<Manager> GetInternal(Expression<Func<Data.Labels.Manager, bool>> conditions)
        => _context.Managers
            .Where(conditions)
            .Select(DbManagerConverter.ToManager());


    private readonly Context _context;
    private readonly ILabelService _labelService;
}
