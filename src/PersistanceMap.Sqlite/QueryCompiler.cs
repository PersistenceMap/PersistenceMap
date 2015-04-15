using PersistanceMap.QueryParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.Sqlite
{
    public class QueryCompiler : PersistanceMap.QueryCompiler
    {
        protected override void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write(string.Format("CREATE TABLE IF NOT EXISTS {0} (", part.Compile()));
        }
    }
}
