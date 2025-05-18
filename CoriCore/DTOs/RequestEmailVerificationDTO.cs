using System;

namespace CoriCore.DTOs;

public class RequestEmailVerificationDTO
{
    public string FullName { get; set; }
    public string Email { get; set; }
}
