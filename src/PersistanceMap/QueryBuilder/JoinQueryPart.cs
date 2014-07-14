using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class JoinQueryPart<T> : SelectExpressionQueryPart<T>, ISelectExpressionQueryPart
    {
        public JoinQueryPart(string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
            : this(null, entity, mapOperations)
        {
        }

        public JoinQueryPart(string identifier, string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
            : base(identifier, entity, mapOperations)
        {
        }

        public override string Compile()
        {
            //return string.Format("join {0} on {1}", Entity, base.Compile());
            return string.Format("join {0}{1}{2}", Entity, string.IsNullOrEmpty(Identifier) ? string.Empty : string.Format(" {0}", Identifier) , base.Compile());
        }

        public override string ToString()
        {
            return string.Format("join {0}", Entity);
        }

        internal void AddOperations(IEnumerable<IExpressionMapQueryPart> operations)
        {
            operations.ForEach(o => Operations.Add(o));
        }
    }
}
