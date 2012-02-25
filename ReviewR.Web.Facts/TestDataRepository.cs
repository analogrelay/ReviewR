using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ReviewR.Web.Models;

namespace ReviewR.Web.Facts.Authentication
{
    public class TestDataRepository : IDataRepository
    {
        private TestDbSet<Role> _roles = new TestDbSet<Role>();
        private TestDbSet<User> _users = new TestDbSet<User>();

        public IDbSet<Role> Roles { get { return _roles; } }
        public IDbSet<User> Users { get { return _users; } }

        public int SaveChanges()
        {
            _roles.Save();
            _users.Save();
            return 0;
        }
    }
}
