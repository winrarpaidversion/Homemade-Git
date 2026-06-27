using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher) 
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login);

            if(user == null)
            {
                throw new Exception("User not found");
            }

            var isPasswordValid = await _passwordHasher.VerifyPasswordAsync(request.Password, user.PasswordHash);

            if(!isPasswordValid)
            {
                throw new Exception("Invalid login or password");
            }

            return new LoginResponse
            {
                UserId = user.Id,
                Login = user.Login
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var exists = await _userRepository.ExistsByLoginAsync(request.Login);

            if(exists)
            {
                throw new Exception("User already exists.");
            }

            var passwordHash = await _passwordHasher.HashPasswordAsync(request.Password);

            var user = new User()
            {
                Login = request.Login,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);

            return new RegisterResponse
            {
                UserId = user.Id,
                Login = user.Login
            };
        }
    }
}
