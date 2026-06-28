using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using HomemadeGit.Core.Services;
using HomemadeGit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class UserStore : IUserStore
    {
        private readonly AppDbContext _dbContext;

        public UserStore(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsByLoginAsync(string login)
        {
            return await _dbContext.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
