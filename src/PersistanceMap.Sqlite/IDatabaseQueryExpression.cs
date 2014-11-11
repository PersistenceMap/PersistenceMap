
using System;
using System.Linq.Expressions;
namespace PersistanceMap.Sqlite
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        //void Create();

        ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : IQueryExpression
    {
        void Create();

        void Alter();

        ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field);

        /// <summary>
        /// Marks a column to be a primary key column
        /// </summary>
        /// <param name="key">The field that marks the key</param>
        /// <param name="isAutoIncrement">Is the column a auto incrementing column</param>
        /// <returns></returns>
        ITableQueryExpression<T> Key(Expression<Func<T, object>> key, bool isAutoIncrement = false);

        /// <summary>
        /// Marks a set of columns to be a combined primary key of a table
        /// </summary>
        /// <param name="keyFields">Properties marking the primary keys of the table</param>
        /// <returns></returns>
        ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keyFields);

        /// <summary>
        /// Marks a field to be a foreignkey column
        /// </summary>
        /// <typeparam name="TRef">The referenced table for the foreign key</typeparam>
        /// <param name="field">The foreign key field</param>
        /// <param name="reference">The key field in the referenced table</param>
        /// <returns></returns>
        ITableQueryExpression<T> ForeignKey<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference);

        /// <summary>
        /// Drops the key definition.
        /// </summary>
        /// <param name="keyFields">All items that make the key</param>
        /// <returns></returns>
        ITableQueryExpression<T> DropKey(params Expression<Func<T, object>>[] keyFields);

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="field">The field to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        ITableQueryExpression<T> Field(Expression<Func<T, object>> field, FieldOperation operation, string precision = null, bool isNullable = true);


        /// <summary>
        /// Drops the table
        /// </summary>
        void Drop();
    }
}
