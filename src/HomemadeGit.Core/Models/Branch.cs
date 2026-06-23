using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class Branch
    {
        public int Id { get; set; }

        public Repository Repository { get; set; }
        public int RepositoryId { get; set; }

        public Commit? HeadCommit { get; set; }
        public int? HeadCommitId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
