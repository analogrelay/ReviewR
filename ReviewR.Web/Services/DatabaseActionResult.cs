using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewR.Web.Services
{
    public enum DatabaseActionOutcome
    {
        ObjectNotFound,
        Forbidden,
        Conflict,
        Success
    }

    public class DatabaseActionResult<T>
    {
        public DatabaseActionOutcome Outcome { get; private set; }
        public T Object { get; private set; }

        public DatabaseActionResult(DatabaseActionOutcome outcome, T result)
        {
            Outcome = outcome;
            Object = result;
        }

        public static DatabaseActionResult<T> NotFound()
        {
            return new DatabaseActionResult<T>(DatabaseActionOutcome.ObjectNotFound, default(T));
        }

        public static DatabaseActionResult<T> Forbidden()
        {
            return new DatabaseActionResult<T>(DatabaseActionOutcome.Forbidden, default(T));
        }

        public static DatabaseActionResult<T> Conflict()
        {
            return new DatabaseActionResult<T>(DatabaseActionOutcome.Conflict, default(T));
        }

        public static DatabaseActionResult<T> Success(T value)
        {
            return new DatabaseActionResult<T>(DatabaseActionOutcome.Success, value);
        }
    }
}