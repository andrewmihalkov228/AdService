using AdsWebService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsWebService.Controllers
{
    [Route("api/platforms")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IAdsPlatformService _service;
        public AdsController(IAdsPlatformService service)
        {
            _service = service;
        }

        [HttpPost("load")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> LoadPlatformsFromFile(IFormFile file) 
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            using (var stream = file.OpenReadStream())
            {
                await _service.LoadPlatformsAsync(stream);
            }

            return Ok(new { message = $"Файл '{file.FileName}' успешно загружен.", size = file.Length });
        }

        [HttpGet("search")]
        public IActionResult SearchPlatformsForLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Параметр 'location' не может быть пустым.");
            }

            var platforms = _service.SearchPlatforms(location);

            return Ok(platforms);
        }
    }
}
