using Microsoft.Extensions.Logging;

namespace Core.Extensions.Logging;

internal static partial class LogMessages
{
    [LoggerMessage(1000, LogLevel.Warning, "An artist with the Spotify ID '{artistId}' wasn't found.")]
    public static partial void LogSpotifyArtistIdWasNotFound(this ILogger logger, string artistId);

    [LoggerMessage(1001, LogLevel.Warning, "A release with the Spotify ID '{releaseId}' wasn't found.")]
    public static partial void LogSpotifyReleaseIdWasNotFound(this ILogger logger, string releaseId);


    [LoggerMessage(1050, LogLevel.Warning, "Can't add the artist with the Spotify ID '{spotifyArtistId}'.")]
    public static partial void LogArtistWasNotAdded(this ILogger logger, string spotifyArtistId);
}
