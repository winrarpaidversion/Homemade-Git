using HomemadeGit.Core.DTOs.Repositories;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IRepositoryService
    {
        // create, delete, search
        Task<RepositoryResponse> CreateRepositoryAsync(int userId,CreateRepositoryRequest request);
        Task<RepositoryResponse> UpdateRepositoryAsync(int userId, int repositoryId, UpdateRepositoryRequest request);
        Task DeleteRepositoryAsync(int userId, int repositoryId);
        Task<List<RepositoryListItemResponse>> SearchRepositoriesAsync(int userId, SearchRepositoriesRequest request);
        Task<RepositoryResponse> GetRepositoryByIdAsync(int userId, int repositoryId);
    }
}
