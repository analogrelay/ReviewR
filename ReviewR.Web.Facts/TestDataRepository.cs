using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models;

namespace ReviewR.Web.Facts
{
    public class TestDataRepository : IDataRepository
    {
        private int _nextId = 0;

        private TestEntitySet<Role> _roles;
        private TestEntitySet<User> _users;
        private TestEntitySet<Review> _reviews;

        public IEntitySet<Role> Roles { get { return _roles; } }
        public IEntitySet<User> Users { get { return _users; } }
        public IEntitySet<Review> Reviews { get { return _reviews; } }

        public int LastId { get { return _nextId - 1; } }

        public TestDataRepository()
        {
            _roles = new TestEntitySet<Role>(this);
            _users = new TestEntitySet<User>(this);
            _reviews = new TestEntitySet<Review>(this);
        }

        public int SaveChanges()
        {
            _roles.Save();
            _users.Save();
            _reviews.Save();
            return 0;
        }

        internal int GetId()
        {
            return _nextId++;
        }
    }
}
