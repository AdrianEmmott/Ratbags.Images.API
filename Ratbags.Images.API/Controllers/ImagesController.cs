using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;

namespace Ratbags.Images.API.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{filename}")]
        [SwaggerOperation(Summary = "Gets an image",
        Description = "Returns an image if it exists in wwwroot")]
        public IActionResult Get(string filename)
        {
            // makes sure the images folder matches case of the actual folder
            // as docker (linux) won't like discrepencies
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
