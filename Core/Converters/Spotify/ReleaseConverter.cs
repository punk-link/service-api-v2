using Core.Data.Releases;

namespace Core.Converters.Spotify;

internal static class ReleaseConverter
{
    public static Release ToDbRelease(this SpotifyDataExtractor.Models.Releases.Release release, in DateTime timeStamp)
    {
        return new Release
        {
            Created = timeStamp,
            Description = string.Empty,
            FeaturingArtists = new List<Data.Artists.Artist>(),
            Id = 0,
            ImageDetails = "",
            Label = release.Label,
            Name = release.Name,
            ReleaseArtists = new List<Data.Artists.Artist>(),
            ReleaseDate = timeStamp, //release.ReleaseDate,
            SpotifyId = release.Id,
            TrackNumber = release.TrackNumber,
            Tracks = "",
            Upc = release.ExternalIds.Upc,
            Updated = timeStamp
        };
    }


    public static List<Release> ToDbReleases(this IEnumerable<SpotifyDataExtractor.Models.Releases.Release> releases)
    {
        var results = new List<Release>(releases.Count());
        foreach (var release in releases)
            results.Add(ToDbRelease(release));

        return results;
    }
}
