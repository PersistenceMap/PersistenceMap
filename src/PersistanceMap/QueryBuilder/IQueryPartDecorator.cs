using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    public interface IQueryPartDecorator : IQueryPart
    {
        void Add(IQueryPart part);

        void AddToLast(IQueryPart part, MapOperationType operation);

        void AddToLast(IQueryPart part, Func<IQueryPart, bool> predicate);

        IEnumerable<IQueryPart> Parts { get; }
    }
}
