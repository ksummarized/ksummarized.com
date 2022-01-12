using System;

namespace api.Data
{
    public class AuthResultVM
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}