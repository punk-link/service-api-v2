
using Core.Data.Artists;

namespace Core.Converters.Artists;

internal static class DbArtistConverter
{
    public static Models.Artists.Artist ToArtist(this Artist dbArtist) 
        => new()
        {
            Id = dbArtist.Id,
            ImageDetails = dbArtist.ImageDetails.ToImageDetails(),
            LabelId = dbArtist.LabelId,
            Name = dbArtist.Name,
            Releases = Enumerable.Empty<Models.Releases.Release>().ToList()
        };


    public static Artist ToIdOnlyDbArtist(this Artist dbArtist) 
        => new()
        {
            Id = dbArtist.Id,
            SpotifyId = dbArtist.SpotifyId
        };


    public static Models.Artists.SlimArtist ToSlimArtist(this Artist dbArtist) 
        => new()
        {
            Id = dbArtist.Id,
            Name = dbArtist.Name
        };
}
