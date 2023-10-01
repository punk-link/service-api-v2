using FluentValidation;
using FluentValidation.Results;

namespace Core.Models.Labels.Validators;

internal class LabelValidator : AbstractValidator<Label>
{
    public static ValidationResult ValidateName(string? labelName)
    {
        var trimmedLabelName = labelName?.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedLabelName))
            return new ValidationResult();

        return new ValidationResult(new List<ValidationFailure>(1)
        {
            new(nameof(Label.Name), "Label's name not provided.")
        });
    }
}
