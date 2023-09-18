using FluentValidation;
using FluentValidation.Results;

namespace Core.Models.Labels.Validators;

internal class ManagerValidator : AbstractValidator<Manager>
{
    public static ValidationResult ValidateName(string? managerName)
    {
        var trimmedManagerName = managerName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedManagerName))
        {
            Manager manager;

            return new ValidationResult(new List<ValidationFailure>(1)
            {
                new(nameof(manager.Name), "Manager's name not provided.")
            });
        }

        return new ValidationResult();
    }
}
