using FluentValidation;
using FluentValidation.Results;

namespace Core.Models.Labels.Validators;

internal class ManagerContextValidator : AbstractValidator<ManagerContext>
{
    public ValidationResult ValidateArtistBelongsToLabel(ManagerContext context, int artistLabelId)
    {
        if (context.LabelId == default) 
            return new ValidationResult(new List<ValidationFailure>(1)
            {
                new(nameof(context.LabelId), "The manager isn't attached to any labels.")
            });

        if (context.LabelId != artistLabelId) 
            return new ValidationResult(new List<ValidationFailure>(1)
            {
                new(nameof(context.LabelId), "The artist already added to another label.")
            });

        return Validate(context);
    }
}
