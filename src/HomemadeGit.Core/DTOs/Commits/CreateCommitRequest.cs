using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Commits
{
    public class CreateCommitRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public List<CreateCommitFileRequest> Files { get; set; } = new();
    }

    public class CreateCommitFileRequest
    {
        public string Path { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
