using Core.Data;
using Core.Data.Artists;
using Core.Data.Common;
using Core.Services.Releases;
using Microsoft.Extensions.Logging;

namespace CoreTests.Services.Releases;

public class ReleaseServiceTests
{
    public ReleaseServiceTests()
    {
        var releasesMock = _releases.BuildMock().BuildMockDbSet();

        var contextMock = Substitute.For<Context>();
        contextMock.Releases.Returns(releasesMock);

        var loggerFactoryMock = Substitute.For<ILoggerFactory>();
        
        _sut = new ReleaseService(contextMock, loggerFactoryMock);
    }


    /*[Fact]
    public async Task Add_Should()
    {
        var result = await _sut.Add(new List<Release>(), DateTime.Now);

        Assert.True(false, "This test needs an implementation");
    }*/


    [Fact]
    public async Task Count_ShouldReturnActualReleaseCount()
    {
        var result = await _sut.Count();

        Assert.Equal(_releases.Count, result);
    }


    [Fact]
    public async Task Get_ShouldReturnNoneReleaseWhenIdIsNotMatch()
    {
        var result = await _sut.Get(0);

        Assert.True(result.HasNoValue);
    }


    [Fact]
    public async Task Get_ShouldReturnRelease()
    {
        var result = await _sut.Get(ExistingReleaseId);

        Assert.True(result.HasValue);
        Assert.Equal(ExistingReleaseId, result.Value.Id);
        Assert.Equal(ExistingReleaseName, result.Value.Name);
    }


    [Fact]
    public async Task GetSlim_ShouldReturnEmptyWhenArtistIdIsNotMatch()
    {
        var results = await _sut.GetSlim(0);

        Assert.Empty(results);
    }


    [Fact]
    public async Task GetSlim_ShouldReturnReleases()
    {
        var results = await _sut.GetSlim(ExistingArtistId);

        Assert.NotEmpty(results);
        Assert.Equal(_releases.Count(x => x.ReleaseArtists.Any(y => y.Id == ExistingArtistId)), results.Count);
    }


    [Fact]
    public async Task GetUpcContainersToUpdate_ShouldReturnEmptyWhenAllReleasesAreUpdated()
    {
        var results = await _sut.GetUpcContainersToUpdate(DateTime.Parse("2019-01-01"), 0);

        Assert.Empty(results);
    }


    [Fact]
    public async Task GetUpcContainersToUpdate_ShouldReturnNonUpdatedContainers()
    {
        var timeStamp = DateTime.Parse("2022-01-01");
        var results = await _sut.GetUpcContainersToUpdate(timeStamp, 0);

        Assert.NotEmpty(results);
        Assert.Equal(_releases.Count(x => x.Updated <= timeStamp), results.Count);
    }


    private const int ExistingArtistId = 1;
    private const int ExistingReleaseId = 1;
    private const string ExistingReleaseName = "My Release";

    private static readonly List<Core.Data.Artists.Artist> Artists = new()
    {
        new()
        {
            Id = ExistingArtistId
        }
    };
    
    private readonly List<Core.Data.Releases.Release> _releases = new()
    {
        new()
        {
            Id = ExistingReleaseId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            Name = ExistingReleaseName,
            ReleaseArtists = Artists,
            Updated = DateTime.Parse("2020-10-20")
        },
        new()
        {
            Id = 2,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            Name = "My Second Release",
            ReleaseArtists = Artists,
            Updated = DateTime.Parse("2022-10-20")
        }
    };


    private readonly IReleaseService _sut;
}