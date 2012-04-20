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
        private MockEntitySet<Participant> _participants;
        private MockEntitySet<Iteration> _iterations;
        private MockEntitySet<Token> _tokens;
        private MockEntitySet<Credential> _credentials;

        public IEntitySet<Role> Roles { get { return _roles; } }
        public IEntitySet<User> Users { get { return _users; } }
        public IEntitySet<Review> Reviews { get { return _reviews; } }
        public IEntitySet<FileChange> Changes { get { return _changes; } }
        public IEntitySet<Comment> Comments { get { return _comments; } }
        public IEntitySet<Participant> Participants { get { return _participants; } }
        public IEntitySet<Iteration> Iterations { get { return _iterations; } }
        public IEntitySet<Token> Tokens { get { return _tokens; } }
        public IEntitySet<Credential> Credentials { get { return _credentials; } }

        public int LastId { get { return _nextId - 1; } }

        public MockDataRepository()
        {
            _roles = new MockEntitySet<Role>(this);
            _users = new MockEntitySet<User>(this);
            _reviews = new MockEntitySet<Review>(this);
            _changes = new MockEntitySet<FileChange>(this);
            _comments = new MockEntitySet<Comment>(this);
            _participants = new MockEntitySet<Participant>(this);
            _iterations = new MockEntitySet<Iteration>(this);
            _tokens = new MockEntitySet<Token>(this);
            _credentials = new MockEntitySet<Credential>(this);
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
