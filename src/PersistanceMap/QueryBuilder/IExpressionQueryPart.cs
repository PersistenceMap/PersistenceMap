using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    public interface IExpressionQueryPart : IQueryPart
    {
        //TODO: Rename this interface to better suit the purpose!
        //TODO: This does not have expressions it has QueryMaps!
        IEnumerable<IQueryMap> Operations { get; }
    }
}
