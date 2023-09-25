using Core.Data;
using Core.Models.Labels;
using Core.Services.Labels;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CoreTests;

public class LabelServiceTest
{
    public LabelServiceTest() 
    { 
        var labelsMock = _labels.BuildMock().BuildMockDbSet();

        var contextMock = Substitute.For<Context>();
        contextMock.Labels.Returns(labelsMock);

        contextMock.Labels
            .When(x => x.AddAsync(Arg.Any<Core.Data.Labels.Label>(), Arg.Any<CancellationToken>()))
            .Do(parameters =>
            {
                var label = parameters.Arg<Core.Data.Labels.Label>();
                label.Id = AddedLabelId;
            });

        contextMock.SaveChangesAsync(Arg.Any<CancellationToken>());

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
    public async Task Add_ShouldReturnAddedLabel()
    {
        var sut = new LabelService(_context);

        var result = await sut.Add(AddedLabelName);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedLabelName, result.Value.Name);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultIfLabelNotFound()
    {
        var sut = new LabelService(_context);

        var result = await sut.Get(DefaultAddingManagerContext, 0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultWhenManagerLabelIsNotMatch()
    {
        var sut = new LabelService(_context);

        var result = await sut.Get(DifferentLabelManagerContext, AddedLabelId);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnLabel()
    {
        var sut = new LabelService(_context);

        var result = await sut.Get(DefaultAddingManagerContext, AddedLabelId);

        Assert.NotEqual(default, result);
        Assert.Equal(AddedLabelId, result.Id);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Modify_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var sut = new LabelService(_context);
        var label = new Label
        {
            Id = ModifiedLabelId,
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = labelName
#pragma warning restore CS8601 // Possible null reference assignment.
        };
        
        var result = await sut.Modify(DefaultModifyingManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenLabelNotFound()
    {
        var sut = new LabelService(_context);
        var label = new Label
        {
            Id = 0,
            Name = ModifiedLabelName
        };
        
        var result = await sut.Modify(DefaultModifyingManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerLabelIsNotMatch()
    {
        var sut = new LabelService(_context);
        var label = new Label
        {
            Id = ModifiedLabelId,
            Name = ModifiedLabelName
        };
        
        var result = await sut.Modify(DifferentLabelManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnModifiedLabel()
    {
        var sut = new LabelService(_context);
        var label = new Label
        {
            Id = ModifiedLabelId,
            Name = ModifiedLabelName
        };
        
        var result = await sut.Modify(DefaultModifyingManagerContext, label);

        Assert.True(result.IsSuccess);
        Assert.Equal(ModifiedLabelName, result.Value.Name);
    }


    private const int AddedLabelId = 1;
    private const string AddedLabelName = "My label";
    
    private const int ModifiedLabelId = 2;
    private const string ModifiedLabelName = "My modified label";

    private static readonly ManagerContext DefaultAddingManagerContext = new()
    {
        Id = 1,
        LabelId = 1
    };

    private static readonly ManagerContext DefaultModifyingManagerContext = new()
    {
        Id = 1,
        LabelId = 2
    };

    private static readonly ManagerContext DifferentLabelManagerContext = new()
    {
        Id = 1,
        LabelId = 3
    };

    private readonly List<Core.Data.Labels.Label> _labels = new()
    {
        new()
        {
            Id = AddedLabelId,
            Name = AddedLabelName
        },
        new()
        {
            Id = ModifiedLabelId,
            Name = AddedLabelName
        }
    };

    private readonly Context _context;
}