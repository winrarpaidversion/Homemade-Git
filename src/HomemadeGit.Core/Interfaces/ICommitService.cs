using System;
using System.Collections.Generic;
using System.Text;
using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.DTOs.Branches;
using HomemadeGit.Core.DTOs.Commits;

namespace HomemadeGit.Core.Interfaces
{
    public interface ICommitService
    {
        Task<CommitResponse> CreateCommitAsync(int userId, int repositoryId, int branchId, CreateCommitRequest request);

        Task<List<CommitListItemResponse>> GetRepositoryCommitsAsync(int userId, int repositoryId);

        Task<CommitResponse> GetCommitByIdAsync(int userId, int commitId);

        Task<RepositorySnapshotResponse> GetCommitSnapshotAsync(int userId, int commitId);

        Task<RepositorySnapshotResponse> CloneRepositoryAsync(int userId, int repositoryId, int? branchId);

        Task ResetBranchToCommitAsync(int userId, int repositoryId, int branchId, ResetBranchRequest request);
    }
}
