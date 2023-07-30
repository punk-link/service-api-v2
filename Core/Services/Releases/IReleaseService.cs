using Core.Models.Releases;
using CSharpFunctionalExtensions;

namespace Core.Services.Releases;

public interface IReleaseService
{
    public Task<Result> Add(List<SpotifyDataExtractor.Models.Releases.Release> releases, DateTime timeStamp, CancellationToken cancellationToken = default);
    public Task<List<SlimRelease>> GetSlim(int artistId, CancellationToken cancellationToken = default);
}
