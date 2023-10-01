using FluentValidation;
using FluentValidation.Results;

namespace Core.Models.Labels.Validators;

internal class ManagerValidator : AbstractValidator<Manager>
{
    public static ValidationResult ValidateName(string? managerName)
    {
        var trimmedManagerName = managerName?.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedManagerName))
            return new ValidationResult();

        return new ValidationResult(new List<ValidationFailure>(1)
        {
            new(nameof(Manager.Name), "Manager's name not provided.")
        });
    }
}
