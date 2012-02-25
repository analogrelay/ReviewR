using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Models
{
    public interface IDataRepository
    {
        IDbSet<Role> Roles { get; }
        IDbSet<User> Users { get; }
        int SaveChanges();
    }

    public class DefaultDataRepository : DbContext, IDataRepository
    {
        public virtual IDbSet<Role> Roles { get; set; }
        public virtual IDbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasMany<Role>(u => u.Roles)
                        .WithMany(r => r.Users)
                        .Map(m => m.ToTable("UserRoles"));
        }
    }
}