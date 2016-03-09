using PersistenceMap.Interception;
using System;
using System.Collections.Generic;
using PersistenceMap.QueryBuilder;

namespace PersistenceMap.UnitTest
{
    public class MockedContextProvider : IDatabaseContext
    {
        public IConnectionProvider ConnectionProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Commit()
        {
        }

        public void AddQuery(IQueryCommand command)
        {
        }

        public ISettings Settings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IQueryCommand> QueryStore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public InterceptorCollection Interceptors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public QueryKernel Kernel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<T> Execute<T>(CompiledQuery query)
        {
            throw new NotImplementedException();
        }

        public void Execute(CompiledQuery query)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
