using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
    }
}
