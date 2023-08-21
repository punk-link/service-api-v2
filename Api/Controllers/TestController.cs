using Microsoft.AspNetCore.Mvc;
using SpotifyDataExtractor;

namespace Api.Controllers
{
    [Route("api/v{version:apiVersion}/tests")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController(IArtistService artistService)
        {
            _artistService = artistService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _artistService.Get(id);
            return Ok(result);
        }


        [HttpGet("ex")]
        public async Task<IActionResult> GetEx()
        {
            throw new Exception("text");
        }


        private readonly IArtistService _artistService;
    }
}
