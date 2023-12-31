﻿using Microsoft.Extensions.Caching.Memory;
using SpotifyDataExtractor.Constants;
using System.Text.Json;
using SpotifyDataExtractor.Models.Common;
using System.Collections.Concurrent;
using System.Net;

namespace SpotifyDataExtractor;

public class SpotifyClient : ISpotifyClient
{
    public SpotifyClient(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
    }


    public async Task<List<T>> Get<T>(IEnumerable<string> urls, CancellationToken cancellationToken = default) where T : struct
    {
        var bag = new ConcurrentBag<T>();
        await Parallel.ForEachAsync(urls, cancellationToken, async (url, ct) => 
        {
            var result = await MakeRequest<T>(HttpMethod.Get, url, ct);
            bag.Add(result);
        });

        return bag.ToList();
    }


    public Task<T> Get<T>(string url, CancellationToken cancellationToken = default) where T : struct 
        => MakeRequest<T>(HttpMethod.Get, url, cancellationToken);


    private async ValueTask<string> GetAccessToken(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue(SpotifyTokenCacheKey, out string? token))
            return token!;

        var spotifyToken = await RequestToken(cancellationToken);
        var cacheDuration = GetDateTimeOffset(spotifyToken.ExpiresIn);
        _memoryCache.Set(SpotifyTokenCacheKey, spotifyToken.Token, cacheDuration);

        return spotifyToken.Token;    


        static DateTimeOffset GetDateTimeOffset(int spotifyTokenExpirationTime)
        {
            var safetyThreshold = new TimeSpan(0, 1, 0);

            var spotifyTokenLifetime = new TimeSpan(0, 0, spotifyTokenExpirationTime);
            var tokenLifetime = spotifyTokenLifetime - safetyThreshold;
            
            return DateTimeOffset.Now.Add(tokenLifetime);
        }


        static HttpRequestMessage GetRequestMessage() 
            => new(HttpMethod.Post, "token")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>(1)
                {
                    new("grant_type", "client_credentials")
                })
            };


        async Task<SpotifyAccessToken> RequestToken(CancellationToken ct)
        {
            using var request = GetRequestMessage();

            using var httpClient = _httpClientFactory.CreateClient(ClientNames.SpotifyAccountClientName);
            using var response = await httpClient.SendAsync(request, ct);
            
            return await HandleResponse<SpotifyAccessToken>(response, ct);
        }
    }


    private static async Task<T> HandleResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return (await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken))!;
    }


    private async Task<T1> MakeRequest<T1>(HttpMethod method, string url, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessToken(cancellationToken);
        
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {accessToken}");

        using var httpClient = _httpClientFactory.CreateClient(ClientNames.SpotifyApiClientName);
        using var response = await httpClient.SendAsync(request, cancellationToken);

        return await HandleResponse<T1>(response, cancellationToken);
    }

    
    private const string SpotifyTokenCacheKey = "SpotifyTokenCacheKey";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
}