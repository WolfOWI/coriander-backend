using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoriCore.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Uploads an image file to the uploads directory
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <returns>The relative path to the uploaded image</returns>
        Task<string> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Deletes an image from the uploads directory by its filename
        /// </summary>
        /// <param name="fileName">The name of the file to delete (e.g. "abc123.jpg")</param>
        /// <returns>True if the file was successfully deleted, false otherwise</returns>
        Task<bool> DeleteImageAsync(string fileName);

        /// <summary>
        /// Removes a user's profile picture by setting it to null and deleting the image file
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>True if the profile picture was removed successfully, false if the user was not found</returns>
        Task<bool> RemoveUserProfilePictureAsync(int userId);

        /// <summary>
        /// Updates a user's profile picture, removing the old one if it exists
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="file">The new profile picture file</param>
        /// <returns>The relative path to the uploaded image, or null if the user was not found</returns>
        Task<string?> UpdateUserProfilePictureAsync(int userId, IFormFile file);
    }
}
