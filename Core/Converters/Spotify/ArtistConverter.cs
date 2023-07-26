using Core.Data.Artists;

namespace Core.Converters.Spotify;

internal static class ArtistConverter
{
    public static Artist ToDbArtist(this SpotifyDataExtractor.Models.Artists.Artist artist, int labelId, in DateTime timeStamp)
    {
        return new Artist
        {
            Created = timeStamp,
            //ImageDetails
            LabelId = labelId,
            Name = artist.Name,
            SpotifyId = artist.Id,
            Updated = timeStamp
        };
    }
}
