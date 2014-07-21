
using System.Collections.Generic;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public interface IQueryPartsMap
    {
        //IEnumerable<IQueryMap> Mappings { get; }

        void Add(IQueryPart map);

        CompiledQuery Compile();
    }
}
