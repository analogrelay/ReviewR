using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using ReviewR.Web.Infrastructure;

namespace ReviewR.Web.Facts
{
    internal interface IMockEntitySet
    {
        void Save();
    }

    public class MockEntitySet<T> : IMockEntitySet, IEntitySet<T> where T : class
    {
        private IList<T> _items = new List<T>();
        private IList<Tuple<bool, T>> _pending = new List<Tuple<bool, T>>();
        private MockDataRepository _repo;

        public MockEntitySet(MockDataRepository repo)
        {
            _repo = repo;
        }

        public IEntityQuery<T> Include(string path)
        {
            // Don't do anything special here. Just verify the include path
            VerifyObjectPath(typeof(T), path);
            return this;
        }

        public T Add(T entity)
        {
            _pending.Add(Tuple.Create(/* add/modify */ true, entity));
            return entity;
        }

        public T Remove(T entity)
        {
            _pending.Add(Tuple.Create(/* remove */ false, entity));
            return entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _items.AsQueryable().ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _items.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _items.AsQueryable().Provider; }
        }

        void IMockEntitySet.Save()
        {
            foreach (var action in _pending)
            {
                if (action.Item1)
                {
                    int id = _repo.GetId();
                    action.Item2.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance).SetValue(action.Item2, id, new object[0]);
                    _items.Add(action.Item2);
                }
                else
                {
                    _items.Remove(action.Item2);
                }
            }
            _pending.Clear();
        }

        private static void VerifyObjectPath(Type type, string path)
        {
            string[] components = path.Split('.');
            Type current = type;
            foreach(string component in components)
            {
                PropertyInfo prop = current.GetProperty(component, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    throw new InvalidOperationException("Invalid Include Path: " + path);
                }
                current = prop.PropertyType;
            }
        }
    }
}
