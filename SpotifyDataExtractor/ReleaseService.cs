using SpotifyDataExtractor.Models.Releases;

namespace SpotifyDataExtractor;

public class ReleaseService : IReleaseService
{
    public ReleaseService(ISpotifyClient spotifyClient) 
    {
        _spotifyClient = spotifyClient;    
    }


    public async Task<List<Release>> Get(IEnumerable<string>? ids, CancellationToken cancellationToken = default)
    {
        var idList = ids is null ? Enumerable.Empty<string>().ToList() : ids.ToList();
        if (!idList.Any())
            return Enumerable.Empty<Release>().ToList();

        var urls = idList.Chunk(ReleaseQueryLimit)
            .Select(chunk => {
                var concatenatedIds = string.Join(',', chunk);
                return $"albums?ids={concatenatedIds}";
            });

        return await _spotifyClient.Get<Release>(urls, cancellationToken);
    }


    public async Task<List<Release>> Get(string artistId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(artistId))
            return Enumerable.Empty<Release>().ToList();

        var firstReleaseContainer = await _spotifyClient.Get<ReleaseContainer>($"artists/{artistId}/albums?limit={ReleaseQueryLimit}", cancellationToken);
        //var firstRelease = JsonSerializer.Serialize(firstReleaseContainer.Items.First());
        if (firstReleaseContainer.Next == string.Empty)
            return firstReleaseContainer.Items;

        if (firstReleaseContainer.Total != default)
            return await GetByTotalNumber(firstReleaseContainer.Items);
        
        return await GetByNextLink(firstReleaseContainer.Items);


        async Task<List<Release>> GetByTotalNumber(List<Release> releases)
        {
            var urlNumber = (firstReleaseContainer.Total / ReleaseQueryLimit);
            var urls = new List<string>(urlNumber);

            var skip = 0;
            for (int i = 0; i < urlNumber; i++)
            {
                skip += ReleaseQueryLimit;
                urls.Add($"artists/{artistId}/albums?limit={ReleaseQueryLimit}&offset={skip}");
            }

            var containers = await _spotifyClient.Get<ReleaseContainer>(urls, cancellationToken);
            foreach (var container in containers)
                releases.AddRange(container.Items);

            return releases;
        }


        async Task<List<Release>> GetByNextLink(List<Release> releases)
        {
            var skip = 0;
            while (true)
            {
                skip += ReleaseQueryLimit;
                var url = $"artists/{artistId}/albums?limit={ReleaseQueryLimit}&offset={skip}";

                var container = await _spotifyClient.Get<ReleaseContainer>(url, cancellationToken);
                releases.AddRange(container.Items);

                if (string.IsNullOrWhiteSpace(container.Next))
                    break;
            }

            return releases;
        }
    }


    private const int ReleaseQueryLimit = 20;

    private readonly ISpotifyClient _spotifyClient;
}
