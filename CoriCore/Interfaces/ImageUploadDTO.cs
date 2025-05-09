using System;
using Microsoft.AspNetCore.Http;

namespace CoriCore.Interfaces;

public class ImageUploadDTO
{
    public required IFormFile File { get; set; }
}
