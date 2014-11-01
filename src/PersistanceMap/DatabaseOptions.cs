using PersistanceMap.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap
{
    public class DatabaseOptions
    {
        public DatabaseOptions()
        {
            LoggerFactory = new LoggerFactory();
        }

        internal ILoggerFactory LoggerFactory { get; private set; }
    }
}
