using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class JoinQueryPart<T> : ExpressionQueryPart<T>, IEntityQueryPart
    {
        public JoinQueryPart(string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
            : this(null, entity, mapOperations)
        {
        }

        public JoinQueryPart(string identifier, string entity, IEnumerable<IExpressionMapQueryPart> mapOperations)
            : base(identifier, entity, mapOperations)
        {
        }

        public string Compile()
        {
            //return string.Format("join {0} on {1}", Entity, base.Compile());
            return string.Format("join {0} on {1}", Entity, base.Compile());
        }

        public override string ToString()
        {
            return string.Format("join {0}", Entity);
        }
    }
}
