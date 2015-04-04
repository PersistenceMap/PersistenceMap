using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.UnitTest
{
    public class MockedContextProvider : IDatabaseContext
    {
        public IConnectionProvider ConnectionProvider
        {
            get { throw new NotImplementedException(); }
        }

        public void Commit()
        {
        }

        public void AddQuery(IQueryCommand command)
        {
        }

        public IEnumerable<IQueryCommand> QueryStore
        {
            get { throw new NotImplementedException(); }
        }

        public QueryKernel Kernel
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
        }
    }
}
