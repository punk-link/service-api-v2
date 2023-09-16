using Core.Models.Labels;
using Core.Services.Labels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v{version:apiVersion}/labels")]
[Produces("application/json")]
[ApiController]
public class LabelController : BaseController
{
    public LabelController(ILabelService labelService)
    {
        _labelService = labelService;
    }


    /// <summary>
    /// Gets a lable.
    /// </summary>
    /// <param name="id">Label ID</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Label), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var context = GetManagerContext();
        return OkOrNotFound(await _labelService.Get(context, id));
    }


    /// <summary>
    /// Modifies a lable.
    /// </summary>
    /// <param name="id">Label ID</param>
    /// <param name="label">Modified entity</param>
    /// <returns></returns>
    [HttpPost("{id:int}")]
    [ProducesResponseType(typeof(Label), StatusCodes.Status200OK)]
    public async Task<IActionResult> Modify([FromRoute] int id, [FromBody] Label label)
    {
        var context = GetManagerContext();
        return OkOrBadRequest(await _labelService.Modify(context, label with { Id =  id }));
    }


    private readonly ILabelService _labelService;
}
