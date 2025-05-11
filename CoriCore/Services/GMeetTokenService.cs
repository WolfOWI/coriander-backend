using System;
using System.Text.Json;
using CoriCore.Data;
using CoriCore.Interfaces;
using CoriCore.Models;
using RestSharp;
using Microsoft.Extensions.Options;

namespace CoriCore.Services;

public class GMeetTokenService : IGMeetTokenService
{
    private readonly IRestClient _restClient;
    private readonly GoogleMeetOptions _options;
    private readonly AppDbContext _context;

    public GMeetTokenService(IOptions<GoogleMeetOptions> options, AppDbContext context)
    {
        _restClient = new RestClient("https://oauth2.googleapis.com/token");
        _options = options.Value;
        _context = context;
    }

    public async Task<string> GetAccessTokenAsync(int adminId)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null) throw new Exception("Admin not found");

        if (IsTokenExpired(admin))
        {
            await RefreshTokenAsync(admin);
        }
        return admin.GMeetAccessToken;
    }

    public async Task<GMeetToken> GetTokenAsync(string code, int adminId)
    {
        var admin = await _context.Admins.FindAsync(adminId);
        if (admin == null) throw new Exception("Admin not found");

        var restRequest = new RestRequest();
        restRequest.AddQueryParameter("code", code);
        restRequest.AddQueryParameter("client_id", _options.ClientId);
        restRequest.AddQueryParameter("client_secret", _options.ClientSecret);
        restRequest.AddQueryParameter("redirect_uri", _options.RedirectUrl);
        restRequest.AddQueryParameter("grant_type", "authorization_code");

        var response = await _restClient.PostAsync<GMeetToken>(restRequest);
        await SaveTokenAsync(response, admin);
        return response;
    }

    private async Task<GMeetToken> RefreshTokenAsync(Admin admin)
    {
        var restRequest = new RestRequest();
        restRequest.AddQueryParameter("refresh_token", admin.GMeetRefreshToken);
        restRequest.AddQueryParameter("client_id", _options.ClientId);
        restRequest.AddQueryParameter("client_secret", _options.ClientSecret);
        restRequest.AddQueryParameter("grant_type", "refresh_token");

        var response = await _restClient.PostAsync<GMeetToken>(restRequest);
        response.refresh_token = admin.GMeetRefreshToken;
        await SaveTokenAsync(response, admin);
        return response;
    }

    private async Task SaveTokenAsync(GMeetToken token, Admin admin)
    {
        admin.GMeetAccessToken = token.access_token;
        admin.GMeetRefreshToken = token.refresh_token;
        admin.GMeetTokenGeneratedAt = DateTime.UtcNow;
        admin.GMeetTokenExpiresIn = token.expires_in;

        await _context.SaveChangesAsync();
    }

    private bool IsTokenExpired(Admin admin)
    {
        if (admin.GMeetTokenGeneratedAt == null || admin.GMeetTokenExpiresIn == null)
            return true;

        return admin.GMeetTokenGeneratedAt.Value.AddSeconds(admin.GMeetTokenExpiresIn.Value) <= DateTime.UtcNow;
    }
}
