using HomemadeGit.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IAuthService
    {
        // Register, Login, 
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
    }
}
