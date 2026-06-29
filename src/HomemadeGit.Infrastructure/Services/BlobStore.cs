using HomemadeGit.Core.Interfaces;
using HomemadeGit.Core.Models;
using HomemadeGit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Infrastructure.Services
{
    public class BlobStore : IBlobStore
    {
        private readonly AppDbContext _ctx;

        public BlobStore(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Blob blob)
        {
            await _ctx.Blobs.AddAsync(blob);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Blob?> GetByHashAsync(string hash)
        {
            return await _ctx.Blobs.FirstOrDefaultAsync(b => b.Hash == hash);
        }
    }
}
