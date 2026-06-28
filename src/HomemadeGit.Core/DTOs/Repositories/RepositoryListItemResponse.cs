using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Repositories
{
    public class RepositoryListItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerLogin { get; set; } = string.Empty;
        public bool isPublic { get; set; }
    }
}
