using HomemadeGit.Core.DTOs.Branches;
using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Services
{
    public class BranchService : IBranchService
    {
        private readonly IRepositoryStore _repositoryStore;
        private readonly IBranchStore _branchStore;

        public BranchService(IRepositoryStore repositoryStore, IBranchStore branchStore)
        {
            _repositoryStore = repositoryStore;
            _branchStore = branchStore;
        }

        public async Task<BranchResponse> CreateBranchAsync(int userId, int repositoryId, CreateBranchRequest request)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
            {
                throw new Exception("repository not found");
            }

            if (!CanWrite(repository, userId))
                throw new Exception("DENIED");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("branch name is required");

            var existingBranch = await _branchStore.GetByNameAsync(repositoryId, request.Name.Trim());

            if (existingBranch == null)
            {
                throw new Exception("branch with this name already exists");
            }

            Branch? sourceBranch;

            if (request.SourceBranchId.HasValue)
                sourceBranch = await _branchStore.GetByIdAsync(request.SourceBranchId.Value);
            else
                sourceBranch = await _branchStore.GetDefaultBranchAsync(repositoryId);

            if (sourceBranch == null)
                throw new Exception("source branch not found");

            if (sourceBranch.RepositoryId != repositoryId)
                throw new Exception("source branch belongs to another repository");

            var branch = new Branch
            {
                RepositoryId = repositoryId,
                Name = request.Name.Trim(),
                HeadCommitId = sourceBranch.HeadCommitId
            };

            await _branchStore.AddAsync(branch);

            var savedBranch = await _branchStore.GetByIdAsync(branch.Id);

            return MapToResponse(savedBranch ?? branch, repository.DefaultBranchId);
        }

        public async Task<List<BranchResponse>> GetBranchesAsync(int userId, int repositoryId)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception("repository not found");

            if (!CanRead(repository, userId))
                throw new Exception("DENIED");

            var branches = await _branchStore.GetByRepositoryIdAsync(repositoryId);

            return branches.Select(branch => MapToResponse(branch, repository.DefaultBranchId)).ToList();
        }

        private BranchResponse MapToResponse(Branch branch, int? defaultBranchId)
        {
            return new BranchResponse
            {
                Id = branch.Id,
                RepositoryId = branch.RepositoryId,
                Name = branch.Name,
                HeadCommitId = branch.HeadCommitId,
                HeadCommitHash = branch.HeadCommit?.Hash,
                IsDefault = branch.Id == defaultBranchId,
            };
        }

        private bool CanRead(Repository repository, int userId)
        {
            return repository.isPublic ||
               repository.OwnerId == userId ||
               repository.UserRepositories.Any(ur => ur.UserId == userId);
        }

        private bool CanWrite(Repository repository, int userId)
        {
            return repository.OwnerId == userId || repository.UserRepositories.Any(ur => ur.UserId == userId);
        }

    }
}
