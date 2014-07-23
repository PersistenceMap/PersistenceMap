using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.QueryProvider
{
    public class MapOption<T>
    {
        public IQueryMap MapTo<TOut>(Expression<Func<T, TOut>> source, string alias)
        {
            throw new NotImplementedException();

            //return new PredicateQueryPart(MapOperationType.Include,
            //    () =>
            //    {
            //        return string.Format("{0} as {1}", source, FieldHelper.ExtractPropertyName(alias));
            //    });
        }

        public IQueryMap MapTo<TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            throw new NotImplementedException();

            //return new PredicateQueryPart(MapOperationType.Include,
            //    () =>
            //    {
            //        return string.Format("{0} as {1}", source, FieldHelper.ExtractPropertyName(alias));
            //    });
        }

        public IQueryMap MapTo<TAlias, TOut>(Expression<Func<T, TOut>> source, Expression<Func<TAlias, TOut>> alias)
        {
            throw new NotImplementedException();
            //return new PredicateQueryPart(MapOperationType.Include,
            //    () =>
            //    {
            //        return string.Format("{0} as {1}", FieldHelper.ExtractPropertyName(source), FieldHelper.ExtractPropertyName(alias));
            //    });
        }
    }
}
