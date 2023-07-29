using Microsoft.Extensions.Logging;

namespace Core.Logging;

internal static partial class LogMessages
{
    [LoggerMessage(1000, LogLevel.Warning, "An artist with a Spotify ID '{artistId}' wasn't found.")]
    public static partial void LogSpotifyArtistIdWasntFound(this ILogger logger, string artistId);
}
