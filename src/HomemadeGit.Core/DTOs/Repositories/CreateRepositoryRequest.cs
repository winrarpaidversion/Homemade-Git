using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Repositories
{
    public class CreateRepositoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool isPublic { get; set; }

    }
}
