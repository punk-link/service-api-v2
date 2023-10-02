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


    public static List<Data.Common.ImageDetails> ToDbImageDetails(this IEnumerable<SpotifyDataExtractor.Models.Common.ImageDetails>? imageDetails, string altText)
    {
        if (imageDetails is null)
            return Enumerable.Empty<Data.Common.ImageDetails>().ToList();

        return imageDetails.Select(x => x.ToDbImageDetails(altText))
            .ToList();
    }


    public static Models.ImageDetails ToImageDetails(this SpotifyDataExtractor.Models.Common.ImageDetails imageDetails)
        => new()
            {
                AltText = string.Empty,
                Height = imageDetails.Height,
                Url = imageDetails.Url,
                Width = imageDetails.Width
            };


    public static List<Models.ImageDetails> ToImageDetails(this IEnumerable<SpotifyDataExtractor.Models.Common.ImageDetails>? imageDetails)
    {
        if (imageDetails is null)
            return Enumerable.Empty<Models.ImageDetails>().ToList();

        return imageDetails.Select(ToImageDetails)
            .ToList();
    }
}
