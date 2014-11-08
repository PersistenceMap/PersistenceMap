
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

        void Update();

        //ITableQueryExpression<T> AddField(string field, string dbType, bool nullable = true);

        //ITableQueryExpression<T> DropField(string field);

        //ITableQueryExpression<T> AlterField(string field);

        ITableQueryExpression<T> Ignore(Expression<Func<T, object>> field);

        ITableQueryExpression<T> Key(params Expression<Func<T, object>>[] keys);

        ITableQueryExpression<T> Key<TRef>(Expression<Func<T, object>> field, Expression<Func<TRef, object>> reference, string name = null);
    }
}
