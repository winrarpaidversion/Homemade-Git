using HomemadeGit.Core.DTOs.Branches;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IBranchService
    {
        Task<List<BranchResponse>> GetBranchesAsync(int userId, int repositoryId);

        Task<BranchResponse> CreateBranchAsync(int userId, int repositoryId, CreateBranchRequest request);
    }
}
