using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Model.Entities.Profile;
using LGED.Model.Entities.Student;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LGED.Model.Context
{
    public class LgedDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRoles,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public LgedDbContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Company> Company { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TestStudent> TestStudent { get; set; }
        public DbSet<StudentImage> StudentImage { get; set; }
    }
}