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
    public class TestEntitySet<T> : IEntitySet<T> where T : class
    {
        private IList<T> _items = new List<T>();
        private IList<Tuple<bool, T>> _pending = new List<Tuple<bool, T>>();
        private TestDataRepository _repo;

        public TestEntitySet(TestDataRepository repo)
        {
            _repo = repo;
        }

        public IEntityQuery<T> Include(string path)
        {
            // Don't do anything special here.
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

        internal void Save()
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
        }
    }
}
