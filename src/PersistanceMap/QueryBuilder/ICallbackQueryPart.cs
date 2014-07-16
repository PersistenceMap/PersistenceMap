using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public interface ICallbackQueryPart : IQueryPart
    {
        LambdaExpression Callback { get; set; }
    }
}
