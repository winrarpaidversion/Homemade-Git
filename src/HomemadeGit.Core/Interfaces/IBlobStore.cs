using HomemadeGit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Interfaces
{
    public interface IBlobStore
    {
        Task<Blob?> GetByHashAsync(string hash);
        Task AddAsync(Blob blob);
    }
}
