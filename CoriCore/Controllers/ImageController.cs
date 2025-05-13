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

        /// <summary>
        /// Removes a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A success message if the profile picture was removed, or an error if the user wasn't found</returns>
        [HttpDelete("profile-picture/{userId}")]
        public async Task<IActionResult> RemoveProfilePicture(int userId)
        {
            try
            {
                var result = await _imageService.RemoveUserProfilePictureAsync(userId);
                if (result)
                {
                    return Ok(new { message = "Profile picture removed successfully" });
                }
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
