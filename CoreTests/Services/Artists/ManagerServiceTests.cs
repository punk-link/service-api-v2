using Core.Data;
using Core.Models.Labels;
using Core.Services.Labels;
using CoreTests.Shared;

namespace CoreTests.Services.Artists;

public class ManagerServiceTests
{
    public ManagerServiceTests()
    {
        var managerMock = _managers.BuildMock().BuildMockDbSet();

        var contextMock = Substitute.For<Context>();
        contextMock.Managers.Returns(managerMock);

        contextMock.Managers
            .When(x => x.AddAsync(Arg.Any<Core.Data.Labels.Manager>(), Arg.Any<CancellationToken>()))
            .Do(parameters =>
            {
                var manager = parameters.Arg<Core.Data.Labels.Manager>();
                manager.Id = AddedManagerId;
            });

        contextMock.SaveChangesAsync(Arg.Any<CancellationToken>());

        var labelServiceMock = Substitute.For<ILabelService>();
        labelServiceMock.Add(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(parameters =>
            {
                var labelName = parameters.Arg<string>();

                return Result.Success(new Label
                {
                    Id = ManagerContexts.DefaultAddingManagerContext.Id,
                    Name = labelName
                });
            });

        _sut = new ManagerService(contextMock, labelServiceMock);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Add_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var manager = new Manager
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = managerName
#pragma warning restore CS8601 // Possible null reference assignment.
        };

        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnAddedManager()
    {
        var manager = new Manager
        {
            Name = AddedManagerName
        };

        var result = await _sut.Add(ManagerContexts.DefaultAddingManagerContext, manager);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedManagerName, result.Value.Name);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task AddMaster_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var result = await _sut.AddMaster(labelName, AddedManagerName);

        Assert.True(result.IsFailure);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task AddMaster_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var result = await _sut.AddMaster(AddedLabelName, managerName);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task AddMaster_ShouldReturnAddedMasterManager()
    {
        var result = await _sut.AddMaster(AddedLabelName, AddedManagerName);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedManagerName, result.Value.Name);
    }


    [Fact]
    public async Task Get_ShouldReturnEmptyListOfManagersWhenMasterManagerIdDoesNotMatch()
    {
        var results = await _sut.Get(ManagerContexts.DifferentLabelManagerContext);

        Assert.NotNull(results);
        Assert.Empty(results);
    }


    [Fact]
    public async Task Get_ShouldReturnManagers()
    {
        var results = await _sut.Get(ManagerContexts.DefaultAddingManagerContext);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal(_managers.Count(x => x.LabelId == ManagerContexts.DefaultAddingManagerContext.LabelId), results.Count);
        foreach (var manager in results)
            Assert.Equal(ManagerContexts.DefaultAddingManagerContext.LabelId, manager.LabelId);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultManagerWhenIdDoesNotMatch()
    {
        var result = await _sut.Get(ManagerContexts.DefaultAddingManagerContext, 0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultManagerWhenManagerContextIdDoesNotMatch()
    {
        var result = await _sut.Get(ManagerContexts.DifferentLabelManagerContext, AddedManagerId);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnManager()
    {
        var result = await _sut.Get(ManagerContexts.DefaultAddingManagerContext, AddedManagerId);

        Assert.Equal(AddedManagerId, result.Id);
    }


    [Fact]
    public async Task GetContext_ShouldReturnDefaultManagerContextWhenIdDoesNotMatch()
    {
        var result = await _sut.GetContext(0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task GetContext_ShouldReturnManagerContext()
    {
        var result = await _sut.GetContext(AddedManagerId);

        Assert.Equal(AddedManagerId, result.Id);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Modify_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = managerName
#pragma warning restore CS8601 // Possible null reference assignment.
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerNotFound()
    {
        var manager = new Manager
        {
            Id = 0,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerContextIdDoesNotMatch()
    {
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };

        var result = await _sut.Modify(ManagerContexts.DifferentLabelManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnModifiedManager()
    {
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };

        var result = await _sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

        Assert.True(result.IsSuccess);
        Assert.Equal(ModifiedManagerName, result.Value.Name);
    }


    private const int AddedManagerId = 1;
    private const string AddedManagerName = "My manager";
    private const string AddedLabelName = "My label";
    private const int ModifiedManagerId = 5;
    private const string ModifiedManagerName = "My modified manager";


    private readonly List<Core.Data.Labels.Manager> _managers = new()
    {
        new()
        {
            Id = AddedManagerId,
            Name = AddedManagerName,
            LabelId = ManagerContexts.DefaultAddingManagerContext.LabelId
        },
        new()
        {
            Id = 2,
            Name = AddedManagerName,
            LabelId = ManagerContexts.DefaultAddingManagerContext.LabelId
        },
        new()
        {
            Id = 3,
            Name = AddedManagerName,
            LabelId = ManagerContexts.DefaultAddingManagerContext.LabelId
        },
        new()
        {
            Id = 4,
            Name = AddedManagerName,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId
        },
        new()
        {
            Id = ModifiedManagerId,
            Name = AddedManagerName,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId
        }
    };

    private readonly ManagerService _sut;
}