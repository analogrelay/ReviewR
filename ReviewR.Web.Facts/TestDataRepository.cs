using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Facts.Authentication
{
    public class TestDataRepository : IDataRepository
    {
        private TestEntitySet<Role> _roles = new TestEntitySet<Role>();
        private TestEntitySet<User> _users = new TestEntitySet<User>();
        private TestEntitySet<Review> _reviews = new TestEntitySet<Review>();

        public IEntitySet<Role> Roles { get { return _roles; } }
        public IEntitySet<User> Users { get { return _users; } }
        public IEntitySet<Review> Reviews { get { return _reviews; } }

        public int SaveChanges()
        {
            _roles.Save();
            _users.Save();
            _reviews.Save();
            return 0;
        }
    }
}
