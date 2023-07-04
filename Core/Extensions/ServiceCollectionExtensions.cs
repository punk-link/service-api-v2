using Core.Services.Artists;
using Core.Services.Presentations;
using Core.Services.Releases;
using Microsoft.Extensions.DependencyInjection;
using SpotifyDataExtractor.Extensions;

namespace Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IArtistService, ArtistService>();
        services.AddTransient<IReleaseStatsService, ReleaseStatsService>();
        services.AddTransient<ISocialNetworkService, SocialNetworkService>();

        services.AddTransient<IPresentationConfigService, PresentationConfigService>();

        services.AddTransient<IReleaseService, ReleaseService>();

        return services;
    }
}
