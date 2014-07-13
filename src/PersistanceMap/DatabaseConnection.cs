using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PersistanceMap.Internals;

namespace PersistanceMap
{
    public class DatabaseConnection
    {
        private IContextProvider _provider;

        public DatabaseConnection(IContextProvider provider)
        {
            _provider = provider;
        }

        public virtual IDbContext Open()
        {
            return new DbContext(_provider);
        }
    }
}
