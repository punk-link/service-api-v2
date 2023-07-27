using Core.Data.Releases;

namespace Core.Converters.Spotify;

internal static class ReleaseConverter
{
    public static Release ToDbRelease(this in SpotifyDataExtractor.Models.Releases.Release release, List<Data.Artists.Artist> releaseArtists, in DateTime timeStamp) 
        => new()
        {
            Created = timeStamp,
            Description = string.Empty,
            ImageDetails = release.ImageDetails.ToDbImageDetails(release.Name),
            Label = release.Label,
            Name = release.Name,
            ReleaseArtists = releaseArtists,
            ReleaseDate = timeStamp, //release.ReleaseDate,
            SpotifyId = release.Id,
            TrackNumber = release.TrackNumber,
            Tracks = Enumerable.Empty<Track>().ToList(),
            Upc = release.ExternalIds.Upc,
            Updated = timeStamp
        };


    public static List<Release> ToDbReleases(this IEnumerable<SpotifyDataExtractor.Models.Releases.Release> releases, 
        List<Data.Artists.Artist> releaseArtists, 
        in DateTime timeStamp = default)
    {
        var results = new List<Release>(releases.Count());
        foreach (var release in releases)
            results.Add(ToDbRelease(release, releaseArtists, timeStamp));

        return results;
    }
}
