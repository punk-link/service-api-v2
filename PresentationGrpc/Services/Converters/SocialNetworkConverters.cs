using Core.Models.Artists;

namespace PresentationGrpc.Services.Converters;

internal static class SocialNetworkConverters
{
    public static List<ArtistSocialNetwork> Convert(List<SocialNetwork> networks) 
        => networks.Select(network => new ArtistSocialNetwork
        {
            Id = network.Id,
            Url = network.Url
        }).ToList();
}
