using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IUpdateQueryProvider
    {
        IUpdateQueryProvider AddToStore();

        IUpdateQueryProvider Update<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null);

        IUpdateQueryProvider Update<T>(Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null);
    }
}
