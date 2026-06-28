using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Repositories
{
    public class RepositoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isPublic { get; set; }

        public int OwnerId { get; set; }
        public string OwnerLogin { get; set; }

        public int? DefaultBranchId { get; set; }
        public string? DefaultBranchName { get; set; }

    }
}
