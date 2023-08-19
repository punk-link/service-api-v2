namespace Core.Converters.Spotify;

internal static class ReleaseTypeConverter
{
    public static Models.Releases.Enums.ReleaseType ToReleaseType(this SpotifyDataExtractor.Models.Enums.ReleaseType releaseType)
    {
        if (!Enum.IsDefined(typeof(Models.Releases.Enums.ReleaseType), (int)releaseType))
            throw new ArgumentOutOfRangeException(nameof(releaseType), $"The value {releaseType} is out of range of the {nameof(releaseType)} enum.");

        return (Models.Releases.Enums.ReleaseType)releaseType;
    }
}
