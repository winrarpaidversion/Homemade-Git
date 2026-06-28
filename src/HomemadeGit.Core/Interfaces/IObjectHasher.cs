using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IObjectHasher
    {
        string HashBytes(byte[] data);

        string HashCommit(string? parentHash, int userId, DateTime createdAt, string title, string description, IEnumerable<CommitHashFile> files);
    }
    public class CommitHashFile
    {
        public string Path { get; set; } = string.Empty;
        public string BlobHash { get; set; } = string.Empty;
    }
}
