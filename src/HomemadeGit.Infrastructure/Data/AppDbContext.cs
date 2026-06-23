using HomemadeGit.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace HomemadeGit.Infrastructure.Data
{
    class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
