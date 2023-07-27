namespace Core.Converters.Spotify;

internal static class ImageDetailsConverter
{
    public static Data.Common.ImageDetails ToDbImageDetails(this in SpotifyDataExtractor.Models.Common.ImageDetails imageDetails, string altText) 
        => new()
        {
            AltText = altText,
            Height = imageDetails.Height,
            Url = imageDetails.Url,
            Width = imageDetails.Width
        };


    public static List<Data.Common.ImageDetails> ToDbImageDetails(this IEnumerable<SpotifyDataExtractor.Models.Common.ImageDetails> imageDetails, string altText) 
        => imageDetails.Select(x => x.ToDbImageDetails(altText))
            .ToList();
}
