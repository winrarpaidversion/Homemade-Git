using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IBranchStore
    {
        Task AddAsync(Branch branch);
        Task UpdateAsync(Branch branch);
        Task DeleteAsync(Branch branch);

        Task<Branch?> GetByIdAsync(int id);
        Task<Branch?> GetByNameAsync(int repositoryId, string name);
        Task<Branch?> GetDefaultBranchAsync(int repositoryId);

        Task<List<Branch>> GetByRepositoryIdAsync(int repositoryId);

        Task UpdateHeadCommitAsync(int branchId, int commitId);
    }
}
