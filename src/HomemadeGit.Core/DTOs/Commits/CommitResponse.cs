using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Commits
{
    public class CommitResponse
    {
        public int Id { get; set; }
        public string Hash { get; set; } = string.Empty;

        public int RepositoryId { get; set; }
        public int? ParentCommitId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public string AuthorLogin { get; set; } = string.Empty;

        public List<CommitFileResponse> Files { get; set; } = new();
    }
}
