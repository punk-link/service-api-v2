using Core.Models.Releases;
using CSharpFunctionalExtensions;

namespace Core.Services.Releases;

public interface IReleaseService
{
	//Add(currentManager labelModels.ManagerContext, artists map[string]artistData.Artist, releases []releaseSpotifyPlatformModels.Release, timeStamp time.Time) error
	//Get(artistId int) ([]artistModels.Release, error)
	//GetCount() int
	//GetMissing(artistId int, artistSpotifyId string) ([]releaseSpotifyPlatformModels.Release, error)
	//GetOne(id int) (artistModels.Release, error)
    public Task<Result> Add(List<SpotifyDataExtractor.Models.Releases.Release> releases, DateTime timeStamp, CancellationToken cancellationToken = default);
    public Task<List<SlimRelease>> GetSlim(int artistId, CancellationToken cancellationToken = default);
    //GetUpcContainersToUpdate(top int, skip int, updateTreshold time.Time) []platformContracts.UpcContainer
    public Task<List<UpcContainer>> GetUpcContainersToUpdate(DateTime updateThreshold, int skip, int top = 100, CancellationToken cancellationToken = default);
	public Task<Result> MarkAsUpdated(IEnumerable<int> ids, DateTime timeStamp, CancellationToken cancellationToken = default);
}
