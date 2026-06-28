using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IRepositoryStore
    {
        Task<Repository?> GetByIdAsync(int id);
        Task<Repository?> GetByNameAsync(int ownerId, string name);

        Task<List<Repository>> SearchByNameAsync(int userId, string query);
        Task<List<Repository>> GetByOwnerIdAsync(int ownerId);

        Task AddAsync(Repository repository);
        Task UpdateAsync(Repository repository);
        Task DeleteAsync(Repository repository);
    }
}
