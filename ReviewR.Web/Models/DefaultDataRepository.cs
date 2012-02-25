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
        IEntitySet<Review> Reviews { get; }
        int SaveChanges();
    }

    public class DefaultDataRepository : IDataRepository
    {
        private ReviewRDbContext _db;

        public virtual IEntitySet<Role> Roles { get; private set; }
        public virtual IEntitySet<User> Users { get; private set; }
        public virtual IEntitySet<Review> Reviews { get; private set; }

        public DefaultDataRepository()
            : base()
        {
            _db = new ReviewRDbContext();
            Users = new DbSetAdaptor<User>(_db.Users);
            Roles = new DbSetAdaptor<Role>(_db.Roles);
            Reviews = new DbSetAdaptor<Review>(_db.Reviews);
        }

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }
    }

    public class ReviewRDbContext : DbContext
    {
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasMany<Role>(u => u.Roles)
                        .WithMany(r => r.Users)
                        .Map(m => m.ToTable("UserRoles"));
        }
    }
}