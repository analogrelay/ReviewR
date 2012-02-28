﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
        private TestEntitySet<FileChange> _changes;
        private TestEntitySet<Comment> _comments;
        private TestEntitySet<Participant> _participants;

        public IEntitySet<Role> Roles { get { return _roles; } }
        public IEntitySet<User> Users { get { return _users; } }
        public IEntitySet<Review> Reviews { get { return _reviews; } }
        public IEntitySet<FileChange> Changes { get { return _changes; } }
        public IEntitySet<Comment> Comments { get { return _comments; } }
        public IEntitySet<Participant> Participants { get { return _participants; } }

        public int LastId { get { return _nextId - 1; } }

        public TestDataRepository()
        {
            _roles = new TestEntitySet<Role>(this);
            _users = new TestEntitySet<User>(this);
            _reviews = new TestEntitySet<Review>(this);
            _changes = new TestEntitySet<FileChange>(this);
            _comments = new TestEntitySet<Comment>(this);
            _participants = new TestEntitySet<Participant>(this);
        }

        public int SaveChanges()
        {
            foreach (var prop in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(p => typeof(ITestEntitySet).IsAssignableFrom(p.FieldType)))
            {
                ITestEntitySet set = (ITestEntitySet)prop.GetValue(this);
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
