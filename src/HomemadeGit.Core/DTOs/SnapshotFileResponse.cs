using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs
{
    public class SnapshotFileResponse
    {
        public string Path { get; set; } = string.Empty;
        public string BlobHash { get; set; } = string.Empty;
        public long Size { get; set; }

        public byte[] Data { get; set;  } = Array.Empty<byte>();
    }
}
