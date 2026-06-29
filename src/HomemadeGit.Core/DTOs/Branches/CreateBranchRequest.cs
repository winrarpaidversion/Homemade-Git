using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.DTOs.Branches
{
    public class CreateBranchRequest
    {
        public string Name { get; set; }  = string.Empty;
        public int? SourceBranchId { get; set; }
    }
}
