using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IAdminService
{
    // Create a new admin
    Task<string> CreateAdmin(AdminDTO adminDTO);

}
