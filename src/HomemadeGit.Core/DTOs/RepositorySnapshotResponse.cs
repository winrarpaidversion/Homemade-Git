using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs
{
    public class RepositorySnapshotResponse
    {
        public int RepositoryId { get; set; }
        public string RepositoryName { get; set; } = string.Empty;

        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;

        public int CommitId { get; set;  }
        public string CommitHash { get; set; } = string.Empty;

        public List<SnapshotFileResponse> Files { get; set; }
    }
}
