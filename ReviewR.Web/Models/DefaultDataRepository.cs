using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.Models
{
    public interface IDataRepository
    {
        IEntitySet<Role> Roles { get; }
        IEntitySet<User> Users { get; }
        int SaveChanges();
    }

    public class DefaultDataRepository : DbContext, IDataRepository
    {
        public virtual IEntitySet<Role> Roles { get; set; }
        public virtual IEntitySet<User> Users { get; set; }

        public DefaultDataRepository()
            : base()
        {
            Users = new DbSetAdaptor<User>(Set<User>());
            Roles = new DbSetAdaptor<Role>(Set<Role>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasMany<Role>(u => u.Roles)
                        .WithMany(r => r.Users)
                        .Map(m => m.ToTable("UserRoles"));
        }
    }
}