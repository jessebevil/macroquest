using System;
using RGAuth.Enums;

namespace RGAuth.Models
{
    public class AuthToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        public long UserId { get; set; }
        public TokenStatus Status { get; set; }
        public string StatusReason { get; set; }

    }
}
