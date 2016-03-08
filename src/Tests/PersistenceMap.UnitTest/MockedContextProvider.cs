using PersistenceMap.Interception;
using System;
using System.Collections.Generic;

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

        public void Dispose()
        {
        }
    }
}
