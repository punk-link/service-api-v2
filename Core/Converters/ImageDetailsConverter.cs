using Core.Data.Common;

namespace Core.Converters;

internal static class DbImageDetailsConverter
{
    public static List<Models.ImageDetails> ToImageDetails(this List<ImageDetails> dbImageDetails)
        => dbImageDetails.Select(ToImageDetails)
            .ToList();


    public static Models.ImageDetails ToImageDetails(this ImageDetails dbImageDetails)
        => new()
        {
            AltText = dbImageDetails.AltText,
            Height = dbImageDetails.Height,
            Width = dbImageDetails.Width,
            Url = dbImageDetails.Url
        };
}
