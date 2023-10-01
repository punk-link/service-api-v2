using Core.Converters.Labels;
using Core.Data;
using Core.Extensions.CSharpFunctionalExtensions;
using Core.Models.Labels;
using Core.Models.Labels.Validators;
using Core.Utils.Time;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Labels;

public class LabelService : ILabelService
{
    public LabelService(Context context, ITimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }


    public async ValueTask<Result<Label>> Add(string? labelName, CancellationToken cancellationToken = default)
    {
        return await Result.Success()
            .EnsureWithValidator(() => LabelValidator.ValidateName(labelName))
            .Map(AddLabel);


        async ValueTask<Label> AddLabel()
        {
            var dbLabel = new Label
            {
                Name = labelName!.Trim()
            }.ToDbLabel(_timeProvider.UtcNow);

            await _context.Labels.AddAsync(dbLabel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetInternal(dbLabel.Id, cancellationToken);
        }
    }


    public async Task<Maybe<Label>> Get(ManagerContext currentManager, int id, CancellationToken cancellationToken = default)
    {
        var label = await GetInternal(id, cancellationToken);
        return label.Id == currentManager.LabelId ? label : Maybe<Label>.None;
    }


    public async ValueTask<Result<Label>> Modify(ManagerContext currentManager, Label label, CancellationToken cancellationToken = default)
    {
        return await Result.Success()
            .EnsureWithValidator(() => LabelValidator.ValidateName(label.Name))
            .Map(GetDbLabel)
            .Ensure(maybeLabel => maybeLabel.HasValue, "The modified label was not found.")
            .Map(maybeLabel => maybeLabel.Value)
            .Ensure(dbLabel => dbLabel.Id == currentManager.LabelId, "The modified managed does not belong to the current label.")
            .Map(ModifyLabel);


        async ValueTask<Maybe<Data.Labels.Label>> GetDbLabel()
        {
            var dbLabel = await _context.Labels
                .Where(x => x.Id == label.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (dbLabel is null)
                return Maybe.None;
        
            return dbLabel;
        }


        async ValueTask<Label> ModifyLabel(Data.Labels.Label dbLabel)
        {
            var trimmedName = label.Name.Trim();
            if (trimmedName == dbLabel.Name)
                return dbLabel.ToLabel();

            dbLabel.Name = trimmedName;
            dbLabel.Updated = _timeProvider.UtcNow;

            _context.Labels.Update(dbLabel);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetInternal(dbLabel.Id, cancellationToken);
        }
    }


    private async ValueTask<Label> GetInternal(int id, CancellationToken cancellationToken) 
        => await _context.Labels
            .Where(x => x.Id == id)
            .Select(x => x.ToLabel())
            .FirstOrDefaultAsync(cancellationToken);


    private readonly Context _context;
    private readonly ITimeProvider _timeProvider;
}
