using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Images.API.Controllers;

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
    [ProducesResponseType(typeof(File), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerOperation(Summary = "Gets an image",
    Description = "Returns an image by filename, if it exists in wwwroot")]
    public IActionResult Get(string filename)
    {
        // makes sure the images folder matches case of the actual folder
        // as docker (linux) won't like discrepencies
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", filename);

        if (!System.IO.File.Exists(filepath))
        {
            return NotFound();
        }

        var image = System.IO.File.OpenRead(filepath);
        string mimeType = GetMimeType(filename);

        return File(image, mimeType);
    }


    // TODO get authorise working
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Creates an image in wwwroot/images",
    Description = "Creates an image wwwroot/images")]
    public async Task<IActionResult> Post()
    {
        var image = Request.Form.Files.FirstOrDefault();

        if (image != null && image.Length > 0)
        {
            var filename = image.FileName;

            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", filename);

            using var stream = new FileStream(filepath, FileMode.Create);
            await image.CopyToAsync(stream); // Copy the uploaded file to the stream

            //return Ok(new { message = $"Image uploaded:", filename = filename });
            return Ok(new { message = $"Image uploaded: {filename}" });
        }

        return BadRequest("Could not upload image");
    }

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
