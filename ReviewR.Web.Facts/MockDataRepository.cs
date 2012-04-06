using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using ReviewR.Web.Infrastructure;
using ReviewR.Web.Models.Data;

namespace ReviewR.Web.Facts
{
    public class MockDataRepository : IDataRepository
    {
        private int _nextId = 0;

        private MockEntitySet<Role> _roles;
        private MockEntitySet<User> _users;
        private MockEntitySet<Review> _reviews;
        private MockEntitySet<FileChange> _changes;
        private MockEntitySet<Comment> _comments;

        public IEntitySet<Role> Roles { get { return _roles; } }
        public IEntitySet<User> Users { get { return _users; } }
        public IEntitySet<Review> Reviews { get { return _reviews; } }
        public IEntitySet<FileChange> Changes { get { return _changes; } }
        public IEntitySet<Comment> Comments { get { return _comments; } }

        public int LastId { get { return _nextId - 1; } }

        public MockDataRepository()
        {
            _roles = new MockEntitySet<Role>(this);
            _users = new MockEntitySet<User>(this);
            _reviews = new MockEntitySet<Review>(this);
            _changes = new MockEntitySet<FileChange>(this);
            _comments = new MockEntitySet<Comment>(this);
        }

        public int SaveChanges()
        {
            foreach (var prop in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(p => typeof(IMockEntitySet).IsAssignableFrom(p.FieldType)))
            {
                IMockEntitySet set = (IMockEntitySet)prop.GetValue(this);
                set.Save();
            }
            return 0;
        }

        internal int GetId()
        {
            return _nextId++;
        }
    }
}
