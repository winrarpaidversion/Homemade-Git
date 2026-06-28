using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using HomemadeGit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class BranchStore : IBranchStore
    {
        private readonly AppDbContext _ctx;

        public BranchStore(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Branch branch)
        {
            await _ctx.Branches.AddAsync(branch);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Branch branch)
        {
            _ctx.Branches.Update(branch);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Branch branch)
        {
            _ctx.Branches.Remove(branch);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Branch?> GetByIdAsync(int id)
        {
            return await _ctx.Branches.Include(b => b.Repository).Include(b => b.HeadCommit).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Branch?> GetByNameAsync(int repositoryId, string name)
        {
            return await _ctx.Branches.FirstOrDefaultAsync(b => b.RepositoryId == repositoryId && b.Name == name);
        }

        public async Task<Branch?> GetDefaultBranchAsync(int repositoryId)
        {
            var defaultBranchId = await _ctx.Repositories.Where(r => r.Id == repositoryId).Select(r => r.DefaultBranchId).FirstOrDefaultAsync();

            if (defaultBranchId == null)
                return null;

            return await _ctx.Branches.Include(b => b.HeadCommit).FirstOrDefaultAsync(b => b.Id == defaultBranchId);
        }

        public async Task<List<Branch>> GetByRepositoryIdAsync(int repositoryId)
        {
            return await _ctx.Branches.Where(b => b.RepositoryId == repositoryId).ToListAsync();
        }

        public async Task UpdateHeadCommitAsync(int branchId, int commitId)
        {
            var branch = await _ctx.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                throw new Exception("Branch not found");

            branch.HeadCommitId = commitId;

            await _ctx.SaveChangesAsync();
        }
    }
}
