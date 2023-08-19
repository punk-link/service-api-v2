using Core.Models.Releases;

namespace Core.Converters.Releases;

internal static class DbReleaseConverter
{
    public static Models.Releases.Release ToRelease(this Data.Releases.Release release)
        => new()
        {
            Id = release.Id,
            ImageDetails = release.ImageDetails.ToImageDetails(),
            Name = release.Name,
            ReleaseDate = DateOnly.FromDateTime(release.ReleaseDate),
            Type = release.Type,
        };


    public static Models.Releases.SlimRelease ToSlimRelease(this Data.Releases.Release release)
        => new()
        {
            Id = release.Id,
            ImageDetails = release.ImageDetails.ToImageDetails(),
            Name = release.Name,
            ReleaseDate = DateOnly.FromDateTime(release.ReleaseDate),
            Type = release.Type,
        };


    public static UpcContainer ToUpcContainer(this Data.Releases.Release release)
        => new()
        {
            Id = release.Id,
            Upc = release.Upc
        };
}
