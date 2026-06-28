using HomemadeGit.Core.DTOs.Repositories;
using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly IRepositoryStore _repositoryStore;
        private readonly IBranchStore _branchStore;

        public RepositoryService(IRepositoryStore repositoryStore, IBranchStore branchStore)
        {
            _repositoryStore = repositoryStore;
            _branchStore = branchStore;
        }

        public async Task<RepositoryResponse> CreateRepositoryAsync(int userId, CreateRepositoryRequest request)
        {
            var existingRepository = await _repositoryStore.GetByNameAsync(userId, request.Name);

            if (existingRepository != null)
                throw new Exception("Repository with this name already exists");

            var repository = new Repository
            {
                OwnerId = userId,
                Name = request.Name,
                Description = request.Description,
                isPublic = request.isPublic,
                CreatedAt = DateTime.UtcNow

            };

            await _repositoryStore.AddAsync(repository);

            var mainBranch = new Branch
            {
                RepositoryId = repository.Id,
                Name = "main",
                HeadCommitId = null
            };

            await _branchStore.AddAsync(mainBranch);

            repository.DefaultBranchId = mainBranch.Id;

            await _repositoryStore.UpdateAsync(repository);

            return MapToResponse(repository);
        }

        public async Task<RepositoryResponse> UpdateRepositoryAsync(int userId, int repositoryId, UpdateRepositoryRequest request)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception("Repository not found");

            if (repository.OwnerId != userId)
                throw new Exception("Access denied");

            if (!string.IsNullOrWhiteSpace(request.Name))
                repository.Name = request.Name;

            if (request.Description != null)
                repository.Description = request.Description;

            if (request.isPublic.HasValue)
                repository.isPublic = request.isPublic.Value;

            await _repositoryStore.UpdateAsync(repository);

            return MapToResponse(repository);
        }

        public async Task DeleteRepositoryAsync(int userId, int repositoryId)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception("Repository not found");

            if (repository.OwnerId != userId)
                throw new Exception("Access denied");

            await _repositoryStore.DeleteAsync(repository);
        }

        public async Task<List<RepositoryListItemResponse>> SearchRepositoriesAsync(int userId, SearchRepositoriesRequest request)
        {
            var repositories = await _repositoryStore.SearchByNameAsync(userId, request.Query);

            return repositories
                .Select(r => new RepositoryListItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    isPublic = r.isPublic,
                    OwnerLogin = r.Owner.Login
                })
                .ToList();
        }

        public async Task<RepositoryResponse> GetRepositoryByIdAsync(int userId, int repositoryId)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception("Repository not found");

            var hasAccess =
                repository.isPublic ||
                repository.OwnerId == userId ||
                repository.UserRepositories.Any(ur => ur.UserId == userId);

            if (!hasAccess)
                throw new Exception("Access denied");

            return MapToResponse(repository);
        }

        private static RepositoryResponse MapToResponse(Repository repository)
        {
            return new RepositoryResponse
            {
                Id = repository.Id,
                Name = repository.Name,
                Description = repository.Description,
                CreatedAt = repository.CreatedAt,
                isPublic = repository.isPublic,
                OwnerId = repository.OwnerId,
                OwnerLogin = repository.Owner?.Login ?? string.Empty
            };
        }
    }
}
