using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    public interface IFieldQueryMap : IEntityQueryPart, IQueryMap, IQueryPart
    {
        string Field { get; }

        string FieldAlias { get; }
    }
}
