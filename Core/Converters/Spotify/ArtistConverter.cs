using Core.Data.Artists;

namespace Core.Converters.Spotify;

internal static class ArtistConverter
{
    public static Artist ToDbArtist(this in SpotifyDataExtractor.Models.Artists.Artist artist, int labelId, in DateTime timeStamp) 
        => new()
        {
            Created = timeStamp,
            ImageDetails = artist.ImageDetails.ToDbImageDetails(artist.Name),
            LabelId = labelId,
            Name = artist.Name,
            SpotifyId = artist.Id,
            Updated = timeStamp
        };
}
