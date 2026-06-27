using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs
{
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
    }
}
