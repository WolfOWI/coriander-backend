using System;
using Microsoft.AspNetCore.Http;

namespace CoriCore.Interfaces;

public class ImageUploadDTO
{
    public IFormFile File { get; set; }
}
