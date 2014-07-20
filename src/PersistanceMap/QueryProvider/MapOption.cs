using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public class MapOption<T>
    {
        public IQueryMap MapTo<T2>(Expression<Func<T, T2>> property, string to)
        {
            throw new NotImplementedException();
        }
    }
}
