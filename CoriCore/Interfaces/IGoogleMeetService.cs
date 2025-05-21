using System.Threading.Tasks;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces
{
    public interface IGoogleMeetService
    {
        Task<GMeetResponse> CreateEventAsync(CreateGMeetEventDTO createGMeetEventDTO, int adminId);
        Task<GMeetResponse> GetEventByIdAsync(string eventId, int adminId);
        Task<GMeetResponse> UpdateEventAsync(string eventId, UpdateGMeetEventDTO updateGMeetEventDTO, int adminId);
        Task DeleteEventAsync(string eventId, int adminId);
    }
}
