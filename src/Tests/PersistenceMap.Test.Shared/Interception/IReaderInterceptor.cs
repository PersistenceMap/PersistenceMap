using System;
using System.Collections.Generic;

namespace PersistenceMap.Interception
{
    public interface IReaderInterceptor
    {
        IReaderInterceptor AddResult<T>(Func<IEnumerable<T>> collection);
    }
}
