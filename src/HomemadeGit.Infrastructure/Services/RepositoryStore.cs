using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using HomemadeGit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class RepositoryStore : IRepositoryStore
    {
        private readonly AppDbContext _ctx;

        public RepositoryStore(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Repository repository)
        {
            await _ctx.Repositories.AddAsync(repository);
            await _ctx.SaveChangesAsync();
        }
        public async Task UpdateAsync(Repository repository)
        {
            _ctx.Repositories.Update(repository);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Repository repository)
        {
            _ctx.Repositories.Remove(repository);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Repository?> GetByIdAsync(int id)
        {
            return await _ctx.Repositories.Include(r => r.Owner).Include(r => r.UserRepositories).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Repository?> GetByNameAsync(int ownerId, string name)
        {
            return await _ctx.Repositories.FirstOrDefaultAsync(r => r.OwnerId == ownerId && r.Name == name);
        }

        public async Task<List<Repository>> GetByOwnerIdAsync(int ownerId)
        {
            return await _ctx.Repositories.Where(r => r.OwnerId == ownerId).ToListAsync();
        }

        public async Task<List<Repository>> SearchByNameAsync(int userId, string query)
        {
            query = query.Trim();

            return await _ctx.Repositories
                .Include(r => r.Owner)
                .Where(r =>
                    r.isPublic ||
                    r.OwnerId == userId ||
                    r.UserRepositories.Any(ur => ur.UserId == userId)).Where(r => string.IsNullOrEmpty(query) || EF.Functions.Like(r.Name, $"%{query}%")).ToListAsync();
        }
    }
}
