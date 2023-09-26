using Core.Data;
using Core.Models.Labels;
using Core.Services.Labels;
using CoreTests.Shared;
using CSharpFunctionalExtensions;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CoreTests;

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
        
        _context = contextMock;

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

        _labelService = labelServiceMock;
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Add_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = managerName
#pragma warning restore CS8601 // Possible null reference assignment.
        };
        
        var result = await sut.Add(ManagerContexts.DefaultAddingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Add_ShouldReturnAddedManager()
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
            Name = AddedManagerName
        };
        
        var result = await sut.Add(ManagerContexts.DefaultAddingManagerContext, manager);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedManagerName, result.Value.Name);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task AddMaster_ShouldReturnFailureWhenLabelNameIsEmpty(string? labelName)
    {
        var sut = new ManagerService(_context, _labelService);
        
        var result = await sut.AddMaster(labelName, AddedManagerName);

        Assert.True(result.IsFailure);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task AddMaster_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var sut = new ManagerService(_context, _labelService);
        
        var result = await sut.AddMaster(AddedLabelName, managerName);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task AddMaster_ShouldReturnAddedMasterManager()
    {
        var sut = new ManagerService(_context, _labelService);
        
        var result = await sut.AddMaster(AddedLabelName, AddedManagerName);

        Assert.True(result.IsSuccess);
        Assert.Equal(AddedManagerName, result.Value.Name);
    }


    [Fact]
    public async Task Get_ShouldReturnEmptyListOfManagersWhenMasterManagerIdDoesNotMatch()
    {
        var sut = new ManagerService(_context, _labelService);

        var results = await sut.Get(ManagerContexts.DifferentLabelManagerContext);

        Assert.NotNull(results);
        Assert.Empty(results);
    }


    [Fact]
    public async Task Get_ShouldReturnManagers()
    {
        var sut = new ManagerService(_context, _labelService);

        var results = await sut.Get(ManagerContexts.DefaultAddingManagerContext);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal(_managers.Count(x => x.LabelId == ManagerContexts.DefaultAddingManagerContext.LabelId), results.Count);
        foreach(var manager in results)
            Assert.Equal(ManagerContexts.DefaultAddingManagerContext.LabelId, manager.LabelId);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultManagerWhenIdDoesNotMatch()
    {
        var sut = new ManagerService(_context, _labelService);

        var result = await sut.Get(ManagerContexts.DefaultAddingManagerContext, 0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnDefaultManagerWhenManagerContextIdDoesNotMatch()
    {
        var sut = new ManagerService(_context, _labelService);

        var result = await sut.Get(ManagerContexts.DifferentLabelManagerContext, AddedManagerId);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task Get_ShouldReturnManager()
    {
        var sut = new ManagerService(_context, _labelService);

        var result = await sut.Get(ManagerContexts.DefaultAddingManagerContext, AddedManagerId);

        Assert.Equal(AddedManagerId, result.Id);
    }


    [Fact]
    public async Task GetContext_ShouldReturnDefaultManagerContextWhenIdDoesNotMatch()
    {
        var sut = new ManagerService(_context, _labelService);

        var result = await sut.GetContext(0);

        Assert.Equal(default, result);
    }


    [Fact]
    public async Task GetContext_ShouldReturnManagerContext()
    {
        var sut = new ManagerService(_context, _labelService);

        var result = await sut.GetContext(AddedManagerId);

        Assert.Equal(AddedManagerId, result.Id);
    }


    [Theory]
    [ClassData(typeof(EmptyStringTestData))]
    public async Task Modify_ShouldReturnFailureWhenManagerNameIsEmpty(string? managerName)
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
#pragma warning disable CS8601 // Possible null reference assignment.
            Name = managerName
#pragma warning restore CS8601 // Possible null reference assignment.
        };
        
        var result = await sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerNotFound()
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
            Id = 0,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };
        
        var result = await sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnFailureWhenManagerContextIdDoesNotMatch()
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };
        
        var result = await sut.Modify(ManagerContexts.DifferentLabelManagerContext, manager);

        Assert.True(result.IsFailure);
    }


    [Fact]
    public async Task Modify_ShouldReturnModifiedManager()
    {
        var sut = new ManagerService(_context, _labelService);
        var manager = new Manager
        {
            Id = ModifiedManagerId,
            LabelId = ManagerContexts.DefaultModifyingManagerContext.LabelId,
            Name = ModifiedManagerName
        };
        
        var result = await sut.Modify(ManagerContexts.DefaultModifyingManagerContext, manager);

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

    private readonly Context _context;
    private readonly ILabelService _labelService;
}