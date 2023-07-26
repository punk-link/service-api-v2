
using Core.Data.Artists;

namespace Core.Converters.Artists;

internal static class DbArtistConverter
{
    public static Models.Artists.Artist ToArtist(this Artist dbArtist)
    {
        return new Models.Artists.Artist
        {
            Id = dbArtist.Id,
            //ImageDetails = x.ImageDetails,
            LabelId = dbArtist.LabelId,
            Name = dbArtist.Name,
            Releases = Enumerable.Empty<Models.Releases.Release>().ToList()
        };
    }


    public static Models.Artists.SlimArtist ToSlimArtist(this Artist dbArtist)
    {
        return new Models.Artists.SlimArtist
        {
            Id = dbArtist.Id,
            Name = dbArtist.Name
        };
    }
}
