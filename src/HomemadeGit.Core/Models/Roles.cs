using System;
using System.Collections.Generic;
using System.Text;

namespace HomemadeGit.Core.Models
{
    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRepository> UserRepositories { get; set; } = new List<UserRepository>();
    }
}
