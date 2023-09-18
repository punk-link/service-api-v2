using Core.Data;
using Core.Services.Labels;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace Tests;

public class LabelServiceTest
{
    public LabelServiceTest() 
    { 
        var labelsMock = new List<Core.Data.Labels.Label>().BuildMock().BuildMockDbSet();

        var contextMock = NSubstitute.Substitute.For<Context>();
        contextMock.Labels.Returns(labelsMock);

        //contextMock.Labels.Received().AddAsync(Arg.Any<Core.Data.Labels.Label>(), Arg.Any<CancellationToken>());

        //contextMock.Received().SaveChangesAsync(Arg.Any<CancellationToken>());

        _context = contextMock;
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Add_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var sut = new LabelService(_context);
        
        var result = await sut.Add(labelName);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnSuccess()
    {
        var sut = new LabelService(_context);
        var labelName = "My lable";

        var result = await sut.Add(labelName);

        Assert.True(result.IsSuccess);
        Assert.Equal(labelName, result.Value.Name);
    }


    private readonly Context _context;
}