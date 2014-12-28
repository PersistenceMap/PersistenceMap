using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IDatabaseQueryExpression : IQueryExpression
    {
        /// <summary>
        /// Creates a create database expression
        /// </summary>
        void Create();

        /// <summary>
        /// Creates a table expression
        /// </summary>
        /// <typeparam name="T">The POCO type defining the table</typeparam>
        /// <returns></returns>
        ITableQueryExpression<T> Table<T>();
    }

    public interface ITableQueryExpression<T> : IQueryExpression
    {
        /// <summary>
        /// Create a create table expression
        /// </summary>
        void Create();

        /// <summary>
        /// Create a alter table expression
        /// </summary>
        void Alter();

        /// <summary>
        /// Drops the table
        /// </summary>
        void Drop();

        /// <summary>
        /// Ignore the field when creating the table
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
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
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="field">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        ITableQueryExpression<T> Column(Expression<Func<T, object>> field, FieldOperation operation = FieldOperation.None, string precision = null, bool? isNullable = null);

        /// <summary>
        /// Creates a expression that is created for operations for a table field
        /// </summary>
        /// <param name="field">The column to alter</param>
        /// <param name="operation">The type of operation for the field</param>
        /// <param name="fieldType">Thy type of the column</param>
        /// <param name="precision">Precision of the field</param>
        /// <param name="isNullable">Is the field nullable</param>
        /// <returns></returns>
        ITableQueryExpression<T> Column(string field, FieldOperation operation = FieldOperation.None, Type fieldType = null, string precision = null, bool? isNullable = null);
    }
}
