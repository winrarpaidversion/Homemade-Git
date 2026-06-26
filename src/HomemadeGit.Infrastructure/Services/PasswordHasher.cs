using BCrypt.Net;
using HomemadeGit.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        const int workFactor = 11;
        public async Task<string> HashPasswordAsync(string password)
        {
            return await Task.Run(() => BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor));
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hashedPassword)
        {
            return await Task.Run(() => BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword));
        }
    }
}
