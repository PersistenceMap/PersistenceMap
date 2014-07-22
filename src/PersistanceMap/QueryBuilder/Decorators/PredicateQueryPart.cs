using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class PredicateQueryPart : IQueryPart
    {
        public PredicateQueryPart(MapOperationType operation, Func<string> predicate)
        {
            MapOperationType = operation;
            Predicate = predicate;
        }

        public Func<string> Predicate { get; private set; }

        public MapOperationType MapOperationType { get; set; }

        public string Compile()
        {
            return Predicate.Invoke();
        }
    }
}
