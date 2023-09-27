using Core.Data;
using Core.Services.Releases;
using Microsoft.Extensions.Logging;

namespace CoreTests.Services.Releases;

public class ReleaseServiceTests
{
    public ReleaseServiceTests()
    {
        var contextMock = Substitute.For<Context>();

        _context = contextMock;

        _loggerFactory = Substitute.For<ILoggerFactory>();

        
        _sut = new ReleaseService(_context, _loggerFactory);
    }


    /*[Fact]
    public async Task AddTest()
    {
        var result = await _sut.Add(new List<Release>(), DateTime.Now);

        Assert.True(false, "This test needs an implementation");
    }*/


    private readonly Context _context;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IReleaseService _sut;
}