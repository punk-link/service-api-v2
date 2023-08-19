namespace Core.Converters.Releases;

internal static class DbReleaseConverter
{
    public static Models.Releases.SlimRelease ToSlimRelease(this Data.Releases.Release release)
        => new()
        {
            Id = release.Id,
            ImageDetails = release.ImageDetails.ToImageDetails(),
            Name = release.Name,
            ReleaseDate = DateOnly.FromDateTime(release.ReleaseDate),
            Type = release.Type,
        };


    public static List<Models.Releases.SlimRelease> ToSlimReleases(this IEnumerable<Data.Releases.Release> releases)
        => releases.Select(ToSlimRelease)
            .ToList();
}
