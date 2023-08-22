using System.Text.Json.Serialization;

namespace SpotifyDataExtractor.Models.Common;

public readonly record struct SpotifyAccessToken
{
    [JsonConstructor]
    public SpotifyAccessToken(string token, string type, int expiresIn) 
    {
        Token = token;
        Type = type;
        ExpiresIn = expiresIn;
    }


    [JsonPropertyName("access_token")]
    public string Token { get; init; } = default!;
    [JsonPropertyName("token_type")]
    public string Type { get; init; } = default!;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
