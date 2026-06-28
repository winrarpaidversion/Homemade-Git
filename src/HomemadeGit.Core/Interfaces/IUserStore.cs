using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IUserStore
    {
        Task<User?> GetByLoginAsync(string login);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task<bool> ExistsByLoginAsync(string login);
    }
}
