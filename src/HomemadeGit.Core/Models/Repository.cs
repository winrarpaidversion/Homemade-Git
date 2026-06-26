using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class Repository
    {
        public int Id { get; set; }

        public User Owner { get; set; }
        public int OwnerId { get; set; }

        public Branch? DefaultBranch { get; set; }
        public int? DefaultBranchId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ICollection<UserRepository> UserRepositories { get; set; } = new List<UserRepository>();
        public ICollection<Commit> Commits { get; set; } = new List<Commit>();
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }
}
