using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using HomemadeGit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class CommitStore : ICommitStore
    {
        private readonly AppDbContext _ctx;

        public CommitStore(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Commit commit)
        {
            await _ctx.Commits.AddAsync(commit);
            await _ctx.SaveChangesAsync();  
        }

        public async Task<Commit?> GetByIdAsync(int id)
        {
            return await _ctx.Commits.Include(c => c.User)
                .Include(c => c.Repository)
                    .ThenInclude(r => r.UserRepositories)
                .Include(c => c.CommitFiles)
                    .ThenInclude(cf => cf.Blob)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Commit>> GetByRepositoryIdAsync(int repositoryId)
        {
            return await _ctx.Commits.Include(c => c.User).Where(c => c.RepositoryId == repositoryId).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
    }
}
