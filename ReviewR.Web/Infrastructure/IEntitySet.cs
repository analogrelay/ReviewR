using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ReviewR.Web.Infrastructure
{
    public interface IEntityQuery<T> : IQueryable<T> where T : class
    {
        IEntityQuery<T> Include(string path);
    }

    public interface IEntitySet<T> : IEntityQuery<T> where T : class
    {
        T Add(T entity);
        T Remove(T entity);
    }

    public class DbQueryAdaptor<T> : IEntityQuery<T> where T : class
    {
        private DbQuery<T> _dbQuery;
        private IQueryable<T> _asQueryable;

        public DbQueryAdaptor(DbQuery<T> dbQuery)
        {
            _dbQuery = dbQuery;
            _asQueryable = _dbQuery as IQueryable<T>;
        }

        public IEntityQuery<T> Include(string path)
        {
            return new DbQueryAdaptor<T>(_dbQuery.Include(path));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _asQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _asQueryable.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _asQueryable.ElementType; }
        }

        public Expression Expression
        {
            get { return _asQueryable.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _asQueryable.Provider; }
        }
    }

    public class DbSetAdaptor<T> : DbQueryAdaptor<T>, IEntitySet<T> where T : class
    {
        private DbSet<T> _dbSet;

        public DbSetAdaptor(DbSet<T> set)
            : base(set)
        {
            _dbSet = set;
        }

        public T Add(T entity)
        {
            return _dbSet.Add(entity);
        }

        public T Remove(T entity)
        {
            return _dbSet.Remove(entity);
        }
    }
}
