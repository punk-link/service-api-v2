namespace PresentationGrpc.Services.Converters;

internal static class ReleaseStatsConverters
{
    public static ReleaseStats Convert(in Core.Models.Artists.ReleaseStats stats)
        => new()
        {
            AlbumNumber = stats.AlbumNumber,
            CompilationNumber = stats.CompilationNumber,
            SingleNumber = stats.SingleNumber
        };
}
