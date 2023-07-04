namespace PresentationGrpc.Services.Converters;

internal static class ImageDetailsConverters
{
    public static ImageDetails Convert(Core.Models.ImageDetails details) 
        => new()
        {
            AltText = details.AltText,
            Height = details.Height,
            Url = details.Url,
            Width = details.Width
        };
}
