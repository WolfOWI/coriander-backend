using System;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IGMeetTokenService
{
    Task<string> GetAccessTokenAsync(int adminId);
    Task<GMeetToken> GetTokenAsync(string code, int adminId);

}
