using PersistenceMap.QueryBuilder.Commands;

namespace PersistenceMap
{
    internal static class DatabaseContextExtensions
    {
        internal static IDeleteQueryExpression AddToStore(this IDeleteQueryExpression expression)
        {
            expression.Context.AddQuery(new QueryCommand(expression.QueryParts));

            return expression;
        }

        internal static IUpdateQueryExpression<T> AddToStore<T>(this IUpdateQueryExpression<T> expression)
        {
            expression.Context.AddQuery(new QueryCommand(expression.QueryParts));

            return expression;
        }

        internal static IInsertQueryExpression<T> AddToStore<T>(this IInsertQueryExpression<T> expression)
        {
            expression.Context.AddQuery(new QueryCommand(expression.QueryParts));

            return expression;
        }
    }
}
