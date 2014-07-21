using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class JoinQueryPart<T> : SelectQueryPart<T>, ISelectQueryPart
    {
        public JoinQueryPart(string entity, IEnumerable<IQueryMap> mapOperations)
            : this(null, entity, mapOperations)
        {
        }

        public JoinQueryPart(string identifier, string entity, IEnumerable<IQueryMap> mapOperations)
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

        internal void AddOperations(IEnumerable<IQueryMap> operations)
        {
            operations.ForEach(o => Operations.Add(o));
        }
    }
}
