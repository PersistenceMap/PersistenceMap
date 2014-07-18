
using System;
using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder
{
    class FromQueryPart<T> : SelectQueryPart<T>, ISelectQueryPart
    {
        public FromQueryPart(string entity)
            : this(entity, null)
        {
        }

        public FromQueryPart(string entity, string identifier)
            : this(entity, identifier, new List<IQueryMap>())
        {
        }

        public FromQueryPart(string entity, string identifier, IEnumerable<IQueryMap> mapOperations)
            : base(identifier, entity, mapOperations)
        {
        }

        public override string Compile()
        {
            if(string.IsNullOrEmpty(Identifier))
                return string.Format("from {0}", Entity);

            return string.Format("from {0} {1}", Entity, Identifier);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Identifier))
                return string.Format("Entity: {0} [{0}]", Entity);

            return string.Format("Entity: {0} [{0} {1}]", Entity, Identifier);
        }

        internal void AddOperation(IQueryMap operation)
        {
            if (operation.MapOperationType != MapOperationType.Include)
                throw new ArgumentException("Only MapOperationType.Include is allowed as operation on a from expression", "operation");

            Operations.Add(operation);
        }
    }
}
