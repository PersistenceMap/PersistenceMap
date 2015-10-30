using System;

namespace PersistenceMap.Sqlite
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        PersistenceMap.Sqlite.ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : PersistenceMap.ITableQueryExpression<T>, IQueryExpression
    {
        /// <summary>
        /// Creates a expression to rename a table
        /// </summary>
        /// <typeparam name="TNew">The type of the new table</typeparam>
        void RenameTo<TNew>();
    }
}
