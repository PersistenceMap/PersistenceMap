using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public interface IProcedureMapOption
    {
        IQueryMap Value<T>(Expression<Func<T>> predicate);

        IQueryMap Value<T>(string name, Expression<Func<T>> predicate);
    }

    public interface IProcedureMapOption<T>
    {
        IQueryMap MapTo<TOut>(string source, Expression<Func<T, TOut>> alias);
    }
}
