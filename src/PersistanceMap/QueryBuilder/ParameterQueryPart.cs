using System;
using System.Collections.Generic;
using System.Linq;

namespace PersistanceMap.QueryBuilder
{
    internal class ParameterQueryPart : IExpressionQueryPart
    {
        public ParameterQueryPart(IEnumerable<IExpressionMapQueryPart> mapOperations)
            : this(null, mapOperations)
        {
        }

        public ParameterQueryPart(string name, IEnumerable<IExpressionMapQueryPart> mapOperations)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
            Name = name;
        }

        IEnumerable<IExpressionMapQueryPart> IExpressionQueryPart.Operations
        {
            get
            {
                return Operations;
            }
        }

        public IList<IExpressionMapQueryPart> Operations { get; private set; }

        public string Name { get; private set; }

        public string Compile()
        {
            throw new NotImplementedException();
        }
    }
}
