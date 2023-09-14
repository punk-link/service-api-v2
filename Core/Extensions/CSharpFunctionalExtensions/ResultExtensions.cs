using CSharpFunctionalExtensions;
using FluentValidation.Results;

namespace Core.Extensions.CSharpFunctionalExtensions
{
    public static class ResultExtensions
    {
        public static Result EnsureWithValidator(this Result result, Func<ValidationResult> func)
        {
            if (result.IsFailure)
                return result;

            var validationResult = func();
            if (validationResult.IsValid)
                return Result.Success();

            return Result.Failure(validationResult.ToCombinedMessage());
        }
    }
}
