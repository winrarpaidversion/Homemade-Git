using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class Commit
    {
        public int Id { get; set; }
        public Repository Repository { get; set; }
        public int RepositoryId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public Commit? ParentCommit { get; set; }
        public int? ParentCommitId { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<CommitFiles> CommitFiles { get; set; } = new List<CommitFiles>();
        
    }
}
