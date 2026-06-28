using System;
using System.Collections.Generic;
using System.Text;
using HomemadeGit.Core.DTOs.Commits;

namespace HomemadeGit.Core.Interfaces
{
    public interface ICommitService
    {
        Task<CommitResponse> CreateCommitAsync(int userId, int repositoryId, CreateCommitRequest request);

        Task<List<CommitListItemResponse>> GetRepositoryCommitsAsync(int userId, int repositoryId);

        Task<CommitResponse> GetCommitByIdAsync(int userId, int commitId);
    }
}
