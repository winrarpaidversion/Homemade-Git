using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<Commit> Commits { get; set; } = new List<Commit>();
        public ICollection<Repository> OwnedRepositories { get; set; } = new List<Repository>();
        public ICollection<UserRepository> UserRepositories { get; set; } = new List<UserRepository>();
    }
}
