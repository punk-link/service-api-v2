using Core.Data;
using Core.Data.Artists;
using Core.Data.Common;
using Core.Services.Artists;
using Core.Services.Releases;
using CoreTests.Shared;
using Microsoft.Extensions.Logging;
using SpotifyDataExtractor.Models.Releases;

namespace CoreTests.Services.Artists;

public class ArtistServiceTests
{
    public ArtistServiceTests()
    {
        var artistsMock = _artists.BuildMock().BuildMockDbSet();
        var releasesMock = _releases.BuildMock().BuildMockDbSet();

        var contextMock = Substitute.For<Context>();
        contextMock.Artists.Returns(artistsMock);
        contextMock.Releases.Returns(releasesMock);

        //contextMock.Artists.FirstOrDefault()

        contextMock.Artists
            .When(x => x.AddAsync(Arg.Any<Artist>(), Arg.Any<CancellationToken>()))
            .Do(parameters =>
            {
                var artist = parameters.Arg<Artist>();
                if (artist.SpotifyId != AddingArtistSpotifyId)
                    return;

                var dbArtist = _artists.First(x => x.Id == AddingArtistId);
                dbArtist.SpotifyId = artist.SpotifyId;
            });

        var loggerFactoryMock = Substitute.For<ILoggerFactory>();

        var releaseServiceMock = Substitute.For<IReleaseService>();

        var spotifyArtistServiceMock = Substitute.For<SpotifyDataExtractor.IArtistService>();
        spotifyArtistServiceMock.Search(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(parameters =>
            {
                var query = parameters.Arg<string>();

                var result = _artistSearchResults
                    .Where(x => x.Name.Contains(query))
                    .ToList();
                
                return Task.FromResult(result);
            });
        spotifyArtistServiceMock.Get(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(parameters =>
            {
                var id = parameters.Arg<string>();

                var result = _artistSpotifyResults
                    .FirstOrDefault(x => x.Id == id);

                return Task.FromResult(result);
            });
        spotifyArtistServiceMock.Get(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<SpotifyDataExtractor.Models.Artists.Artist>()));

        var spotifyReleaseServiceMock = Substitute.For<SpotifyDataExtractor.IReleaseService>();
        spotifyReleaseServiceMock.Get(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Release>()));
        spotifyReleaseServiceMock.Get(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<Release>()));

        _sut = new ArtistService(contextMock, loggerFactoryMock, releaseServiceMock, spotifyArtistServiceMock, spotifyReleaseServiceMock);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Add_ShouldReturnFailureWhenSpotifyIdIsEmpty(string spotifyArtistId)
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, spotifyArtistId);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnFailureWhenSpotifyIdDoesNotFound()
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, "non-existing-spotify-id");

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnFailureWhenArtistWasNotAdded()
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, NonAddingArtistSpotifyId);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldAddArtist()
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, AddingArtistSpotifyId);

        Assert.True(result.IsSuccess);
    }


    [Fact]
    public async Task Add_ShouldReturnFailureWhenArtistLinkedToAnotherLabel()
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, AddingUnlinkedArtistSpotifyId);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldLinkUnlinkedArtist()
    {
        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, AddingLinkedArtistSpotifyId);

        Assert.True(result.IsSuccess);
    }


    [Fact]
    public async Task Get_ShouldReturnNoneWhenIdDoesNotFound()
    {
        var result = await _sut.Get(0);

        Assert.True(result.HasNoValue);
    }


    [Fact]
    public async Task Get_ShouldReturnArtist()
    {
        var result = await _sut.Get(ExistingArtistId);

        Assert.True(result.HasValue);
        Assert.Equal(ExistingArtistName, result.Value.Name);
    }


    [Fact]
    public async Task GetByLabel_ShouldReturnNoneWhenLabelIdDoesNotFound()
    {
        var results = await _sut.GetByLabel(35);

        Assert.Empty(results);
    }


    [Fact]
    public async Task GetByLabel_ShouldReturnArtists()
    {
        var results = await _sut.GetByLabel(ExistingArtistLabelId);

        Assert.NotEmpty(results);
        Assert.Equal(_artists.Count(x => x.LabelId == ExistingArtistLabelId), results.Count);
    }


    [Fact]
    public async Task GetSlim_ShouldReturnNoneWhenArtistIdDoesNotFound()
    {
        var result = await _sut.GetSlim(0);

        Assert.True(result.HasNoValue);
    }


    [Fact]
    public async Task GetSlim_ShouldReturnSlimArtist()
    {
        var result = await _sut.GetSlim(ExistingArtistId);

        Assert.True(result.HasValue);
        Assert.Equal(ExistingArtistName, result.Value.Name);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Search_ShouldReturnEmptyArtistsWhenQueryIsEmpty(string query)
    {
        var results = await _sut.Search(query);

        Assert.Empty(results);
    }


    [Theory]
    [InlineData("M")]
    [InlineData("My")]
    [InlineData("My ")]
    [InlineData(" My ")]
    public async Task Search_ShouldReturnEmptyArtistsWhenQueryIsTooShort(string query)
    {
        var results = await _sut.Search(query);

        Assert.Empty(results);
    }


    [Fact]
    public async Task Search_ShouldReturnEmptyArtistsWhenNameDoesNotExist()
    {
        var results = await _sut.Search("My Non Existing Artist");

        Assert.Empty(results);
    }


    [Fact]
    public async Task Search_ShouldReturnArtists()
    {
        var results = await _sut.Search(ExistingArtistName);

        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Equal(ExistingArtistName, results[0].Name);
    }


    private const int AddingArtistId = 3;
    private const string AddingArtistName = "My Adding Artist";
    private const string AddingArtistSpotifyId = "spotify-id";
    private const int NonAddingArtistId = 4;
    private const string NonAddingArtistName = "My Non Added Artist";
    private const string NonAddingArtistSpotifyId = "non-added-spotify-id";
    private const int ExistingArtistId = 1;
    private const int ExistingArtistLabelId = 1;
    private const string ExistingArtistName = "My Artist";
    private const int NonExistingArtistId = 2;
    private const int NonExistingArtistLabelId = 2;
    private const int AddingLinkedArtistId = 6;
    private const string AddingLinkedArtistName = "My Linked Artist";
    private const string AddingLinkedArtistSpotifyId = "linked-spotify-id";
    private const int AddingUnlinkedArtistId = 5;
    private const string AddingUnlinkedArtistName = "My Unlinked Artist";
    private const string AddingUnlinkedArtistSpotifyId = "unlinked-spotify-id";


    private readonly List<SpotifyDataExtractor.Models.Artists.SlimArtist> _artistSearchResults = new()
    {
        new()
        {
            ImageDetails = Enumerable.Empty<SpotifyDataExtractor.Models.Common.ImageDetails>().ToList(),
            Name = ExistingArtistName
        },
        new()
        {
            Name = "Another Artist"
        },
        new()
        {
            Id = AddingArtistSpotifyId,
            Name = AddingArtistName
        }
    };

    private readonly List<SpotifyDataExtractor.Models.Artists.Artist> _artistSpotifyResults = new()
    {
        new()
        {
            Name = "Another Artist"
        },
        new()
        {
            Id = AddingArtistSpotifyId,
            Name = AddingArtistName
        }
    };

    private readonly List<Artist> _artists = new()
    {
        new()
        {
            Id = ExistingArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = ExistingArtistLabelId,
            Name = ExistingArtistName
        },
        new()
        {
            Id = NonExistingArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = NonExistingArtistLabelId,
            Name = "Non-existing Artist"
        },
        new()
        {
            Id = AddingArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = ExistingArtistLabelId,
            Name = AddingArtistName
        },
        new()
        {
            Id = NonAddingArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = ExistingArtistLabelId,
            Name = NonAddingArtistName
        },
        new()
        {
            Id = AddingUnlinkedArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = ExistingArtistLabelId,
            Name = AddingUnlinkedArtistName
        },
        new()
        {
            Id = AddingLinkedArtistId,
            ImageDetails = Enumerable.Empty<ImageDetails>().ToList(),
            LabelId = default,
            Name = AddingLinkedArtistName,
            SpotifyId = AddingLinkedArtistSpotifyId
        }
    };

    private readonly List<Core.Data.Releases.Release> _releases = new()
    {
        new()
        {
            ReleaseArtists = Enumerable.Empty<Artist>().ToList()
        }
    };


    private readonly ArtistService _sut;
}