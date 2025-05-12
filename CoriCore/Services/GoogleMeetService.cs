using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CoriCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CoriCore.Models;
using RestSharp;
using CoriCore.DTOs;
using System.Security.Cryptography;

namespace CoriCore.Services
{
    public class GoogleMeetService : IGoogleMeetService
    {
        private readonly IRestClient restClient;
        private readonly IGMeetTokenService _gMeetTokenService;
        private readonly GoogleMeetOptions _options;
        private readonly string timeZone = "Africa/Johannesburg";

        public GoogleMeetService(IGMeetTokenService gMeetTokenService, IOptions<GoogleMeetOptions> options)
        {
            restClient = new RestClient("https://www.googleapis.com/calendar/v3/calendars/");
            _gMeetTokenService = gMeetTokenService;
            _options = options.Value;
        }

        public async Task<GMeetResponse> CreateEventAsync(CreateGMeetEventDTO dto, int adminId)
        {
            var restRequest = new RestRequest("primary/events");
            var accessToken = await _gMeetTokenService.GetAccessTokenAsync(adminId);

            // Set default times if not provided
            var startTime = dto.StartTime ?? DateTimeOffset.Now.AddHours(1).RoundToNearestHour();
            var endTime = dto.EndTime ?? 
                         (dto.StartTime?.AddMinutes(dto.DurationMinutes ?? 60) ?? 
                          startTime.AddMinutes(dto.DurationMinutes ?? 60));

            var eventRequest = new
            {
                start = new
                {
                    dateTime = startTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    timeZone = timeZone
                },
                end = new
                {
                    dateTime = endTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    timeZone = timeZone
                },
                summary = dto.Summary,
                description = dto.Description ?? "Google Meet meeting",
                attendees = dto.AttendeeEmails.Select(email => new
                {
                    email = email,
                    responseStatus = "needsAction"
                }).ToList(),
                conferenceData = new
                {
                    createRequest = new
                    {
                        requestId = GenerateRequestId(),
                        conferenceSolutionKey = new
                        {
                            type = "hangoutsMeet"
                        }
                    }
                }
            };

            restRequest.AddHeader("Authorization", $"Bearer {accessToken}");
            restRequest.AddQueryParameter("conferenceDataVersion", "1");
            restRequest.AddJsonBody(eventRequest);

            var response = await restClient.PostAsync<GMeetResponse>(restRequest);
            return response;
        }

        private string GenerateRequestId()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
        }

        public async Task<GMeetResponse> GetEventByIdAsync(string eventId, int adminId)
        {
            var restRequest = new RestRequest($"primary/events/{eventId}");
            var accessToken = await _gMeetTokenService.GetAccessTokenAsync(adminId);

            restRequest.AddHeader("Authorization", $"Bearer {accessToken}");

            var response = await restClient.GetAsync<GMeetResponse>(restRequest);
            return response;
        }

        public async Task<GMeetResponse> UpdateEventAsync(string eventId, UpdateGMeetEventDTO updateGMeetEventDTO, int adminId)
        {
            var restRequest = new RestRequest($"primary/events/{eventId}");
            var accessToken = await _gMeetTokenService.GetAccessTokenAsync(adminId);

            restRequest.AddHeader("Authorization", $"Bearer {accessToken}");
            restRequest.AddJsonBody(updateGMeetEventDTO);

            var response = await restClient.PatchAsync<GMeetResponse>(restRequest);
            return response;
        }

        public async Task DeleteEventAsync(string eventId, int adminId)
        {
            var restRequest = new RestRequest($"primary/events/{eventId}");
            var accessToken = await _gMeetTokenService.GetAccessTokenAsync(adminId);

            restRequest.AddHeader("Authorization", $"Bearer {accessToken}");

            await restClient.DeleteAsync(restRequest);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTimeOffset RoundToNearestHour(this DateTimeOffset dt)
        {
            return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Offset)
                .AddHours(dt.Minute >= 30 ? 1 : 0);
        }
    }
} 