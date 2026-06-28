using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Commits
{
    public class CommitFileResponse
    {
        public string Path { get; set; } = string.Empty;
        public string BlobHash { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
