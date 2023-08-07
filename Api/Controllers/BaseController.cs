using Core.Models.Labels;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
    protected ManagerContext GetManagerContext()
    {
        return new ManagerContext
        {
            Id = 1,
            LabelId = 1
        };
    }


    protected IActionResult OkOrBadRequest<T>(Result<T> result)
    {
        if (result.IsFailure)
            return BadRequest(result.Error);
            
        return Ok(result.Value);
    }


    protected IActionResult OkOrNotFoundOrBadRequest<T>(Result<T> result)
    {
        if (result.IsFailure)
            return BadRequest(result.Error);

        if (result.Value is null || result.Value.Equals(default(T)))
            return NotFound();

        return Ok(result.Value);
    }
}
