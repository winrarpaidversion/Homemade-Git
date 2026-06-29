using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface ICommitStore
    {
        Task AddAsync(Commit commit);
        Task<Commit?> GetByIdAsync(int id);
        Task<List<Commit>> GetByRepositoryIdAsync(int repositoryId);
    }
}
