using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryBuilder
{
    public interface IQueryPartDecorator : IQueryPart
    {
        void Add(IQueryPart queryPart);
    }
}
