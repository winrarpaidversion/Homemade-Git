using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class CommitFiles
    {
        public int Id { get; set; }
        public Commit Commit { get; set; }
        public int CommitId { get; set; }
        public Blob Blob { get; set; }
        public int BlobId { get; set; }
        public string Path { get; set; }

    }
}
