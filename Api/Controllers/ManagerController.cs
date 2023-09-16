using Core.Models.Labels;
using Core.Services.Labels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v{version:apiVersion}/managers")]
[Produces("application/json")]
[ApiController]
public class ManagerController : BaseController
{
    public ManagerController(IManagerService managerService) 
    { 
        _managerService = managerService;
    }


    /// <summary>
    /// Adds a manager to an existing label.
    /// </summary>
    /// <param name="manager">New entity</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Manager), StatusCodes.Status200OK)]
    public async Task<IActionResult> Add([FromBody] Manager manager)
    {
        var context = GetManagerContext();
        return OkOrBadRequest(await _managerService.Add(context, manager));
    }


    /// <summary>
    /// Creates a new lable and adds a master manager to it.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("master")]
    [ProducesResponseType(typeof(Manager), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddMaster([FromBody] MasterManagerRequest request)
    {
        return OkOrBadRequest(await _managerService.AddMaster(request.LabelName, request.ManagerName));
    }


    /// <summary>
    /// Gets all managers for a lable.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<Manager>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var context = GetManagerContext();
        return OkOrNotFound(await _managerService.Get(context));
    }


    /// <summary>
    /// Gets a manager by ID.
    /// </summary>
    /// <param name="id">Manager ID</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Manager), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var context = GetManagerContext();
        return OkOrNotFound(await _managerService.Get(context, id));
    }


    /// <summary>
    /// Modifies an existing manager.
    /// </summary>
    /// <param name="id">Manager ID</param>
    /// <param name="manager">Modified entity</param>
    /// <returns></returns>
    [HttpPost("{id:int}")]
    [ProducesResponseType(typeof(Manager), StatusCodes.Status200OK)]
    public async Task<IActionResult> Modify([FromRoute] int id, [FromBody] Manager manager)
    {
        var context = GetManagerContext();
        return OkOrBadRequest(await _managerService.Add(context, manager with { Id = id }));
    }


    private readonly IManagerService _managerService;
}
