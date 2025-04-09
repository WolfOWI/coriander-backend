using System;

namespace CoriCore.DTOs;

public class RequestEmailVerificationDTO
{
    public string Email { get; set; }
    public string FullName { get; set; }
}
