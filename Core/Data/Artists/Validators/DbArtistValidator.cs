using FluentValidation;
using FluentValidation.Results;

namespace Core.Data.Artists.Validators;

internal class DbArtistValidator : AbstractValidator<Artist>
{
    public static ValidationResult ValidateDoesNotBelongsToLabel(Artist dbArtist)
    {
        if (dbArtist.LabelId != default)
            return new ValidationResult(new List<ValidationFailure>(1)
            {
                new(nameof(Artist.LabelId), "The artist already linked to another label.")
            });

        return new ValidationResult();
    }
}