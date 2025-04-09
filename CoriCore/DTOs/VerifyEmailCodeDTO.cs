using System;

namespace CoriCore.DTOs;

public class VerifyEmailCodeDTO
{
    public string Email { get; set; }
    public string Code { get; set; }
}
