using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Repositories
{
    public class UpdateRepositoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? isPublic { get; set; }
    }
}
