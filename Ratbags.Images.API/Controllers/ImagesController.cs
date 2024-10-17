using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Ratbags.Images.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{filename}")]
        public IActionResult Get(string filename)
        {
            var filepath = Path.Combine("wwwroot", "images", filename);

            if (!System.IO.File.Exists(filepath)) 
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(filepath);
            string mimeType = GetMimeType(filename);

            return File(image, mimeType);
        }

        // TODO HttpPost

        private string GetMimeType(string filename)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
