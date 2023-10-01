using Core.Data;
using Core.Models.Labels;
using Core.Services.Labels;
using Core.Utils.Time;
using CoreTests.Shared;

namespace CoreTests.Services.Labels;

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

        var timeProviderMock = Substitute.For<ITimeProvider>();
        timeProviderMock.UtcNow.Returns(DateTime.UtcNow);

        _sut = new LabelService(contextMock, timeProviderMock);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Add_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var result = await _sut.Add(labelName);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnAddedLabel()
    {
        var result = await _sut.Add(AddedLabelName);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedLabelName, result.Value.Name);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultIfLabelNotFound()
    {
        var result = await _sut.Get(ManagerContexts.DefaultAddingManagerContext, 0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnNoneWhenManagerLabelIsNotMatch()
    {
        var result = await _sut.Get(ManagerContexts.DifferentLabelManagerContext, AddedLabelId);

        Assert.True(result.HasNoValue);
    }


    [Fact]
    public async Task Get_ShouldReturnLabel()
    {
        var result = await _sut.Get(ManagerContexts.DefaultAddingManagerContext, AddedLabelId);

        Assert.True(result.HasValue);
        Assert.Equal(AddedLabelId, result.Value.Id);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Modify_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var label = new Label
        {
            Id = ModifiedLabelId,
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = labelName
#pragma warning restore CS8601 // Possible null reference assignment.
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenLabelNotFound()
    {
        var label = new Label
        {
            Id = 0,
            Name = ModifiedLabelName
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerLabelIsNotMatch()
    {
        var label = new Label
        {
            Id = ModifiedLabelId,
            Name = ModifiedLabelName
        };

        var result = await _sut.Modify(ManagerContexts.DifferentLabelManagerContext, label);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnModifiedLabel()
    {
        var label = new Label
        {
            Id = ModifiedLabelId,
            Name = ModifiedLabelName
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, label);

        Assert.True(result.IsSuccess);
        Assert.Equal(ModifiedLabelName, result.Value.Name);
    }


    private const int AddedLabelId = 1;
    private const string AddedLabelName = "My label";

    private const int ModifiedLabelId = 2;
    private const string ModifiedLabelName = "My modified label";

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

    private readonly LabelService _sut;
}