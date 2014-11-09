
using System;
using System.Linq.Expressions;
namespace PersistanceMap.Sqlite
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        void Create();

        //ITableQueryExpression Table(string table);
        ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : IQueryExpression
    {
        void Create();

        void Alter();

        //ITableQueryExpression<T> AddField(string field, string dbType, bool nullable = true);

        //ITableQueryExpression<T> DropField(string field);

        //ITableQueryExpression<T> AlterField(string field);

        ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field);

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false);

        ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields);

        ITableQueryExpression<T> Key<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference);

        /// <summary>
        /// Drops the key definition.
        /// </summary>
        /// <param name="keyFields">All items that make the key</param>
        /// <returns></returns>
        ITableQueryExpression<T> DropKey(params Expression<Func<T, object>>[] keyFields);

        /// <summary>
        /// Drops the table
        /// </summary>
        void Drop();
    }
}
