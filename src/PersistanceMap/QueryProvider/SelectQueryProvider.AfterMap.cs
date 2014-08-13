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
            throw new NotImplementedException();
        }
    }
}
