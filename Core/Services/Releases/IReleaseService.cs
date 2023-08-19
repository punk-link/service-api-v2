using Core.Models.Releases;
using CSharpFunctionalExtensions;

namespace Core.Services.Releases;

public interface IReleaseService
{
    public Task<Result> Add(List<SpotifyDataExtractor.Models.Releases.Release> releases, DateTime timeStamp, CancellationToken cancellationToken = default);
	public Task<int> Count(CancellationToken cancellationToken = default);
    public Task<Maybe<Release>> Get(int id, CancellationToken cancellationToken = default);
    public Task<List<SlimRelease>> GetSlim(int artistId, CancellationToken cancellationToken = default);
    public Task<List<UpcContainer>> GetUpcContainersToUpdate(DateTime updateThreshold, int skip, int top = 100, CancellationToken cancellationToken = default);
	public Task MarkAsUpdated(IEnumerable<int> ids, DateTime timeStamp, CancellationToken cancellationToken = default);
}
