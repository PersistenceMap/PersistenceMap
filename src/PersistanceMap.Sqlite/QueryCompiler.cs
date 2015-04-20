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
        protected override void CompilePart(IQueryPart part, TextWriter writer, IQueryPartsContainer container, IItemsQueryPart parent = null)
        {
            switch (part.OperationType)
            {
                case OperationType.AddField:
                    throw new NotImplementedException();
                    break;

                default:
                    base.CompilePart(part, writer, container, parent);
                    break;
            }
        }

        protected override void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write(string.Format("CREATE TABLE IF NOT EXISTS {0} (", part.Compile()));
        }
    }
}
