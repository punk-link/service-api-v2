using FluentValidation.Results;

namespace Core.Extensions;

public static class ValidationResultExtensions
{
    public static string ToCombinedMessage(this ValidationResult result)
        => string.Join("; ", result.Errors.Select(x => x.ErrorMessage));
}
