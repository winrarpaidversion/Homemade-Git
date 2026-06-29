using HomemadeGit.Core.DTOs.Commits;
using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Services
{
    public class CommitService : ICommitService
    {
        private readonly IRepositoryStore _repositoryStore;
        private readonly IBranchStore _branchStore;
        private readonly ICommitStore _commitStore;
        private readonly IBlobStore _blobStore;
        private readonly IObjectHasher _objectHasher;
            
        public CommitService(IRepositoryStore repositoryStore, IBranchStore branchStore, ICommitStore commitStore, IBlobStore blobStore, IObjectHasher objectHasher)
        {
            _repositoryStore = repositoryStore;
            _branchStore = branchStore;
            _commitStore = commitStore;
            _blobStore = blobStore;
            _objectHasher = objectHasher;
        }

        public async Task<CommitResponse> CreateCommitAsync(int userId, int repositoryId, int branchId, CreateCommitRequest request)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception();

            if (!CanWrite(repository, userId))
                throw new Exception("DENIED");

            var branch = await _branchStore.GetByIdAsync(branchId);

            if (branch == null)
                throw new Exception("branuch not found");

            if (branch.RepositoryId != repositoryId)
                throw new Exception("branch belongs to another repository");

            var createdAt = DateTime.UtcNow;
            var description = request.Description ?? string.Empty;

            var commitFiles = new List<CommitFiles>();
            var hashFiles = new List<CommitHashFile>();
            var usedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach(var file in request.Files)
            {
                var path = file.Path;

                if (string.IsNullOrWhiteSpace(path))
                    throw new Exception("Path is empty");

                if (!usedPaths.Add(path))
                    throw new Exception($"Duplicate: {path}");

                var blobHash = _objectHasher.HashBytes(file.Data);

                var blob = await _blobStore.GetByHashAsync(blobHash);

                if(blob == null)
                {
                    blob = new Blob
                    {
                        Hash = blobHash,
                        Data = file.Data,
                        Size = file.Data.LongLength
                    };
                    await _blobStore.AddAsync(blob);
                }

                commitFiles.Add(new CommitFiles
                {
                    BlobId = blob.Id,
                    Path =path
                });

                hashFiles.Add(new CommitHashFile
                {
                    Path=path,
                    BlobHash=blobHash
                });
            }
            var parentHash = branch.HeadCommit?.Hash;

            var commitHash = _objectHasher.HashCommit(parentHash, userId, createdAt, request.Title, description, hashFiles);

            var commit = new Commit
            {
                RepositoryId = repositoryId,
                UserId = userId,
                ParentCommitId = branch.HeadCommitId,
                Title = request.Title,
                Description = description,
                CreatedAt = createdAt,
                Hash = commitHash,
                CommitFiles = commitFiles
            };

            await _commitStore.AddAsync(commit);

            await _branchStore.UpdateHeadCommitAsync(branch.Id, commit.Id);

            var savedCommit = await _commitStore.GetByIdAsync(commit.Id);

            if (savedCommit == null)
                throw new Exception("commit was not saved");

            return MapToResponse(savedCommit);
        }

        public async Task<CommitResponse> GetCommitByIdAsync(int userId, int commitId)
        {
            var commit = await _commitStore.GetByIdAsync(commitId);

            if (commit == null)
                throw new Exception("COmmit not found");

            if (!CanRead(commit.Repository, userId))
                throw new Exception("DENIED");

            return MapToResponse(commit);
        }

        public async Task<List<CommitListItemResponse>> GetRepositoryCommitsAsync(int userId, int repositoryId)
        {
            var repository = await _repositoryStore.GetByIdAsync(repositoryId);

            if (repository == null)
                throw new Exception("repository not found");

            if (!CanRead(repository, userId))
                throw new Exception("DENIED");

            var commits = await _commitStore.GetByRepositoryIdAsync(repositoryId);

            return commits.Select(c => new CommitListItemResponse
            {
                Id = c.Id,
                Hash = c.Hash,
                Title = c.Title,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                AuthorLogin = c.User?.Login ?? string.Empty
            }).ToList();
        }

        private bool CanRead(Repository repository, int userId)
        {
            return repository.isPublic || repository.OwnerId == userId || repository.UserRepositories.Any(ur => ur.UserId == userId);
        }
        private bool CanWrite(Repository repository, int userId)
        {
            return repository.OwnerId == userId || repository.UserRepositories.Any(ur => ur.UserId == userId);
        }
        
        private CommitResponse MapToResponse(Commit commit)
        {
            return new CommitResponse
            {
                Id = commit.Id,
                Hash = commit.Hash,
                RepositoryId = commit.RepositoryId,
                ParentCommitId = commit.ParentCommitId,
                Title = commit.Title,
                Description = commit.Description,
                CreatedAt = commit.CreatedAt,
                UserId = commit.UserId,
                AuthorLogin = commit.User?.Login ?? string.Empty,
                Files = commit.CommitFiles.Select(cf => new CommitFileResponse
                {
                    Path = cf.Path,
                    BlobHash = cf.Blob?.Hash ?? string.Empty,
                    Size = cf.Blob?.Size ?? 0
                })
                .OrderBy(f => f.Path)
                .ToList()
            };
        }
    }
}
