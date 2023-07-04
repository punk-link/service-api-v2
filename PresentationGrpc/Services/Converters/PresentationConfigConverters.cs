namespace PresentationGrpc.Services.Converters;

internal static class PresentationConfigConverters
{
    public static PresentationConfig Convert(in Core.Models.Presentations.PresentationConfig config)
    {
        var result = new PresentationConfig();
        result.ShareableSocialNetworkIds.Add(config.ShareableSocialNetworkIds);

        return result;
    }
}
