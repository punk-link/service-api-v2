using Core.Converters.Labels;
using Core.Data;
using Core.Extensions.CSharpFunctionalExtensions;
using Core.Models.Labels;
using Core.Models.Labels.Validators;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
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


    public async ValueTask<Result<Manager>> Add(ManagerContext currentManager, Manager manager, CancellationToken cancellationToken = default)
    {
        var newManager = manager with { LabelId = currentManager.LabelId, Name = manager.Name.Trim() };

        return await Result.Success()
            .EnsureWithValidator(() => ManagerValidator.ValidateName(newManager.Name))
            .Bind(() => AddInternal(manager, DateTime.UtcNow, cancellationToken));
    }


    public async ValueTask<Result<Manager>> AddMaster(string labelName, string managerName, CancellationToken cancellationToken = default)
    {
        return await Result.Success()
            .EnsureWithValidator(() => LabelValidator.ValidateName(labelName))
            .EnsureWithValidator(() => ManagerValidator.ValidateName(managerName))
            .Bind(() => _labelService.Add(labelName, cancellationToken))
            .Bind(AddMaster);


        ValueTask<Result<Manager>> AddMaster(Label label)
        {
            var manager = new Manager
            {
                LabelId = label.Id,
                Name = managerName.Trim(),
            };

            return AddInternal(manager, DateTime.UtcNow, cancellationToken);
        }
    }


    public Task<List<Manager>> Get(ManagerContext currentManager, CancellationToken cancellationToken = default)
        => GetInternal(x => x.LabelId == currentManager.LabelId)
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


    public async ValueTask<Result<Manager>> Modify(ManagerContext currentManager, Manager manager, CancellationToken cancellationToken = default)
    {
        return await Result.Success()
            .EnsureWithValidator(() => ManagerValidator.ValidateName(manager.Name))
            .Map(GetDbManager)
            .Ensure(maybeManager => maybeManager.HasValue, "The modified managed was not found.")
            .Map(maybeManager => maybeManager.Value)
            .Ensure(dbManager => dbManager.LabelId == currentManager.LabelId, "The modified managed does not belong to the current label.")
            .Map(ModifyManager);


        async ValueTask<Maybe<Data.Labels.Manager>> GetDbManager()
        {
            var dbManager = await _context.Managers
                .Where(x => x.Id == manager.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (dbManager is null)
                return Maybe.None;

            return dbManager;
        }


        async ValueTask<Manager> ModifyManager(Data.Labels.Manager dbManager)
        {
            var trimmedName = manager.Name.Trim();
            if (trimmedName == dbManager.Name)
                return dbManager.ToManager();

            dbManager.Name = trimmedName;
            dbManager.Updated = DateTime.UtcNow;

            _context.Managers.Update(dbManager);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetInternal(dbManager.Id, cancellationToken);
        }
    }


    private async ValueTask<Result<Manager>> AddInternal(Manager manager, DateTime timeStamp, CancellationToken cancellationToken)
    {
        var dbManager = manager.ToDbManager(timeStamp);
        await _context.Managers.AddAsync(dbManager, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(await GetInternal(dbManager.Id, cancellationToken));
    }


    private async Task<Manager> GetInternal(int id, CancellationToken cancellationToken)
        => await GetInternal(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    

    private IQueryable<Manager> GetInternal(Expression<Func<Data.Labels.Manager, bool>> conditions)
        => _context.Managers
            .Where(conditions)
            .Select(x => x.ToManager());


    private readonly Context _context;
    private readonly ILabelService _labelService;
}
