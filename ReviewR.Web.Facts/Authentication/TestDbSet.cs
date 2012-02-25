using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ReviewR.Web.Facts.Authentication
{
    public class TestDbSet<T> : IDbSet<T> where T : class
    {
        private IList<T> _items = new List<T>();
        private IList<Tuple<bool, T>> _pending = new List<Tuple<bool, T>>();

        public T Add(T entity)
        {
            _pending.Add(Tuple.Create(/* add/modify */ true, entity));
            return entity;
        }

        public T Attach(T entity)
        {
            throw new NotImplementedException("Not supported by TestDbSet right now");
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException("Not supported by TestDbSet right now");
        }

        public T Create()
        {
            throw new NotImplementedException("Not supported by TestDbSet right now");
        }

        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Not supported by TestDbSet right now");
        }

        public ObservableCollection<T> Local
        {
            get { throw new NotImplementedException("Not supported by TestDbSet right now"); }
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
