using HomemadeGit.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class Sha256ObjectHasher : IObjectHasher
    {
        public string HashBytes(byte[] data)
        {
            var hashBytes = SHA256.HashData(data);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        public string HashCommit(string? parentHash, int userId, DateTime createdAt, string title, string description, IEnumerable<CommitHashFile> files)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"parent:{parentHash ?? "null"}");
            builder.AppendLine($"user:{userId}");
            builder.AppendLine($"createdAt:{createdAt:O}");
            builder.AppendLine($"title:{title}");
            builder.AppendLine($"description:{description}");

            foreach (var file in files.OrderBy(f => f.Path))
            {
                builder.AppendLine($"file:{file.Path}:{file.BlobHash}");
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return HashBytes(bytes);
        }
    }
}
