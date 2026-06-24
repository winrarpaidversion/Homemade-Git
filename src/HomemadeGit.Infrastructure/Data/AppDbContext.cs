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
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<UserRepository> UserRepositories { get; set; }
        public DbSet<Blob> Blobs { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Commit> Commits { get; set; }
        public DbSet<CommitFiles> CommitFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasOne(b => b.Repository).WithMany(r => r.Branches).HasForeignKey(b => b.RepositoryId);
                entity.HasOne(b => b.HeadCommit).WithMany().HasForeignKey(b => b.HeadCommitId);
            });
            modelBuilder.Entity<Commit>(entity =>
            {
                entity.HasOne(c => c.Repository).WithMany(r => r.Commits).HasForeignKey(c => c.RepositoryId);
                entity.HasOne(c => c.User).WithMany(u => u.Commits).HasForeignKey(c => c.UserId);
                entity.HasOne(c => c.ParentCommit).WithMany().HasForeignKey(c => c.ParentCommitId);
            });
            modelBuilder.Entity<CommitFiles>(entity =>
            {
                entity.HasOne(cf => cf.Commit).WithMany(c => c.CommitFiles).HasForeignKey(cf => cf.CommitId);
                entity.HasOne(cf => cf.Blob).WithMany(b => b.CommitFiles).HasForeignKey(cf => cf.BlobId);
            });
            modelBuilder.Entity<Repository>(entity =>
            {
                entity.HasOne(r => r.Owner).WithMany(u => u.OwnedRepositories).HasForeignKey(r => r.OwnerId);
            });
            modelBuilder.Entity<UserRepository>(entity =>
            {
                entity.HasOne(ur => ur.UserRole).WithMany(r => r.UserRepositories).HasForeignKey(ur => ur.UserRoleId);
                entity.HasOne(ur => ur.Repository).WithMany(r => r.UserRepositories).HasForeignKey(ur => ur.RepositoryId);
                entity.HasOne(ur => ur.User).WithMany(u => u.UserRepositories).HasForeignKey(ur => ur.UserId);
            });
        }
    }
}
