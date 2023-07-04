namespace PresentationGrpc.Services.Converters;

internal static class ArtistConverters
{
    public static Artist Convert(in Core.Models.Artists.SlimArtist slimArtist,
        in Core.Models.Presentations.PresentationConfig presentationConfig,
        in Core.Models.Artists.ReleaseStats releaseStats,
        in List<Core.Models.Artists.SocialNetwork> socialNetworks,
        List<Core.Models.Releases.SlimRelease> slimReleases)
    {
        var result = new Artist()
        {
            Id = slimArtist.Id,
            Name = slimArtist.Name,
            PresentationConfig = PresentationConfigConverters.Convert(in presentationConfig),
            ReleaseStats = ReleaseStatsConverters.Convert(in releaseStats),
        };

        var artistSocialNetworks = SocialNetworkConverters.Convert(socialNetworks);
        result.SocialNetworks.Add(artistSocialNetworks);

        var releases = ReleaseConverters.Convert(slimReleases);
        result.Releases.Add(releases);
        
        return result;
    }
}
