using Google.Protobuf.WellKnownTypes;

namespace PresentationGrpc.Services.Converters;

internal static class ReleaseConverters
{
    public static List<SlimRelease> Convert(List<Core.Models.Releases.SlimRelease> releases) 
        => releases.Select(release => new SlimRelease
        {
            Id = release.Id,
            ImageDetails = ImageDetailsConverters.Convert(release.ImageDetails),
            Name = release.Name,
            ReleaseDate = Timestamp.FromDateTime(release.ReleaseDate.ToDateTime(TimeOnly.MinValue)),
            Type = release.Type
        }).ToList();
}
