using Google.Protobuf.WellKnownTypes;

namespace PresentationGrpc.Services.Converters;

internal static class ReleaseConverters
{
    public static List<SlimRelease> Convert(List<Core.Models.Releases.SlimRelease> releases) 
        => releases.Select(release => new SlimRelease
        {
            Id = release.Id,
            // TODO: add a list
            ImageDetails = ImageDetailsConverters.Convert(release.ImageDetails.First()),
            Name = release.Name,
            ReleaseDate = Timestamp.FromDateTime(release.ReleaseDate.ToDateTime(TimeOnly.MinValue)),
            // TODO: add type support
            Type = release.Type.ToString()
        }).ToList();
}
