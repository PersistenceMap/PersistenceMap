using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class ParameterQueryPart : IParameterQueryPart, ICallbackQueryPart, INamedQueryPart, IExpressionQueryPart, IQueryPart
    {
        public ParameterQueryPart(IEnumerable<IMapQueryPart> mapOperations)
            : this(null, mapOperations, null)
        {
        }

        public ParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations)
            : this(name, mapOperations, null)
        {
        }

        public ParameterQueryPart(string name, IEnumerable<IMapQueryPart> mapOperations, LambdaExpression callback)
        {
            // ensure parameter is not null
            mapOperations.EnsureArgumentNotNull("mapOperations");

            Operations = mapOperations.ToList();
            Name = name;
            Callback = callback;
        }

        IEnumerable<IMapQueryPart> IExpressionQueryPart.Operations
        {
            get
            {
                return Operations;
            }
        }

        public IList<IMapQueryPart> Operations { get; private set; }

        public string Name { get; private set; }

        public LambdaExpression Callback { get; set; }

        public string Compile()
        {
            throw new NotImplementedException();
        }
    }
}
