using System;
using System.IO;
using System.Threading.Tasks;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Http;
using CoriCore.Data;
using Microsoft.EntityFrameworkCore;

namespace CoriCore.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public ImageService(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No image file provided");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Console.WriteLine("WebRootPath = " + _env.WebRootPath);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{fileName}";
            return relativePath;
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // Remove any path separators if they were accidentally included
            fileName = Path.GetFileName(fileName);
            
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
            
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveUserProfilePictureAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(user.ProfilePicture))
            {
                // Extract filename from the profile picture path
                var fileName = Path.GetFileName(user.ProfilePicture.TrimStart('/'));
                
                // Delete the actual file
                await DeleteImageAsync(fileName);
                
                // Set the profile picture to null
                user.ProfilePicture = null;
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
