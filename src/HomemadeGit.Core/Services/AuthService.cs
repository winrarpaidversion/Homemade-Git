using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            // to-do: passwordcheck

            return new LoginResponse
            {
                UserId = user.Id,
                Login = user.Login
            };
        }

        public Task RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
