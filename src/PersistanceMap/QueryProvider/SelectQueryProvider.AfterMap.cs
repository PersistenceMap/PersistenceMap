using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IAfterMapQueryProvider<T>, IQueryProvider
    {
        public IAfterMapQueryProvider<T> AfterMap(Action<T> predicate)
        {
            throw new NotImplementedException();
        }

        public IAfterMapQueryProvider<T> Ignore<TIgnore>(Expression<Func<T, TIgnore>> predicate)
        {
            foreach (var part in QueryPartsMap.Parts.Where(p => p.OperationType == OperationType.SelectMap))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var fieldName = FieldHelper.TryExtractPropertyName(predicate);

                var subpart = map.Parts.FirstOrDefault(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == fieldName || ((IFieldQueryMap)f).FieldAlias == fieldName);
                if (subpart != null)
                    map.Remove(subpart);
            }

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }
    }
}
