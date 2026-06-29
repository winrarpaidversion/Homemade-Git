using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Branches
{
    public class BranchResponse
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public string Name { get; set; }  = string.Empty;

        public int? HeadCommitId { get; set; }
        public string? HeadCommitHash { get; set; }

        public bool IsDefault { get; set; }
    }
}
