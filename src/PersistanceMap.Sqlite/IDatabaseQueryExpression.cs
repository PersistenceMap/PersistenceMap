using System;

namespace PersistanceMap.Sqlite
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        PersistanceMap.Sqlite.ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : PersistanceMap.ITableQueryExpression<T>, IQueryExpression
    {
        /// <summary>
        /// Creates a expression to rename a table
        /// </summary>
        /// <typeparam name="TNew">The type of the new table</typeparam>
        void RenameTo<TNew>();
    }
}
