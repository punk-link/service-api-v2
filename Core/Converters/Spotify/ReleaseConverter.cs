using Core.Data.Releases;

namespace Core.Converters.Spotify;

internal static class ReleaseConverter
{
    public static Release ToDbRelease(this SpotifyDataExtractor.Models.Releases.Release release)
    {
        return new Release();
    }


    public static List<Release> ToDbReleases(this IEnumerable<SpotifyDataExtractor.Models.Releases.Release> releases)
    {
        var results = new List<Release>(releases.Count());
        foreach (var release in releases)
            results.Add(ToDbRelease(release));

        return results;
    }
}
