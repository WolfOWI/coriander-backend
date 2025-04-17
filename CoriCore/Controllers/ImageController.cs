using System;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDTO dto)
        {
            try
            {
                var imageUrl = await _imageService.UploadImageAsync(dto.File);
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
