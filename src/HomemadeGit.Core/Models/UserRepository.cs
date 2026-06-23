using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class UserRepository
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Repository Repository { get; set; }
        public Roles UserRole { get; set; }
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RepositoryId { get; set; }
    }
}
