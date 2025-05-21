using System;

namespace CoriCore.DTOs
{
    public class GoogleLoginDTO
    {
        public string IdToken { get; set; } = string.Empty;
        public int Role { get; set; }
    }
}