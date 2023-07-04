using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using SpotifyDataExtractor.Constants;
using SpotifyDataExtractor.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SpotifyDataExtractor.Extensions;

public static class SpotifyDataExtractorExtensions
{
    public static IServiceCollection AddSpotifyDataExtractor(this IServiceCollection services, Action<SpotifyConfig> options)
    {
        services.AddHttpClients(options);

        services.AddTransient<ISpotifyClient, SpotifyClient>();
        services.AddTransient<IArtistService, ArtistService>();
        services.AddTransient<IReleaseService, ReleaseService>();

        return services;
    }


    private static IServiceCollection AddHttpClients(this IServiceCollection services, Action<SpotifyConfig> options)
    {
        var config = GetConfig(options);

        services.AddHttpClient(ClientNames.SpotifyAccountClientName, httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://accounts.spotify.com/api/");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
            var authorizationData = $"Basic {credentials}";

            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), authorizationData);
        }).AddPollyPolicyHandlers();

        services.AddHttpClient(ClientNames.SpotifyApiClientName, httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://api.spotify.com/v1/");

            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddPollyPolicyHandlers();

        return services;


        static SpotifyConfig GetConfig(Action<SpotifyConfig> opts)
        {
            var spotifyConfig = new SpotifyConfig();
            opts.Invoke(spotifyConfig);

            ArgumentException.ThrowIfNullOrEmpty(spotifyConfig.ClientId, nameof(spotifyConfig.ClientId));
            ArgumentException.ThrowIfNullOrEmpty(spotifyConfig.ClientSecret, nameof(spotifyConfig.ClientSecret));

            return spotifyConfig;
        }
    }


    private static IHttpClientBuilder AddPollyPolicyHandlers(this IHttpClientBuilder builder)
        => builder.SetHandlerLifetime(TimeSpan.FromMilliseconds(300_000))
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(500), retryCount: 5))
            );
}
