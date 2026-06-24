using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class Blob
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public byte[] Data { get; set; }

        public ICollection<CommitFiles> CommitFiles { get; set; } = new List<CommitFiles>();
    }
}
