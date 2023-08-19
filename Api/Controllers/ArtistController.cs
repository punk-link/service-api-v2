using Core.Models.Artists;
using Core.Services.Artists;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[Route("api/v{version:apiVersion}/artists")]
[Produces("application/json")]
//[ApiController]
public sealed class ArtistController : BaseController
{
    public ArtistController(IArtistService artistService)
    {
        _artistService = artistService;
    }


    [HttpPost("{spotifyId}")]
    [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
    public async Task<IActionResult> Add([FromRoute][Required] string spotifyId, CancellationToken cancellationToken = default)
    {
        var context = GetManagerContext();

        return OkOrBadRequest(await _artistService.Add(context, spotifyId, cancellationToken));
    }


    [HttpGet("{artistId}")]
    [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute][Required][Range(1, int.MaxValue)] int artistId, CancellationToken cancellationToken = default)
    {
        return OkOrNotFoundOrBadRequest(await _artistService.Get(artistId, cancellationToken));
    }


    private readonly IArtistService _artistService;
}
