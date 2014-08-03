using System;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class PredicateQueryPart : QueryPartDecorator, IQueryPartDecorator, IQueryPart
    {
        public PredicateQueryPart(OperationType operation, Func<string> predicate)
        {
            OperationType = operation;
            Predicate = predicate;
        }
        
        #region IQueryPart Implementation

        public override string Compile()
        {
            var sb = new StringBuilder();

            // compile the predicate
            var value = Predicate.Invoke();
            if (!string.IsNullOrEmpty(value))
                sb.Append(string.Format("{0} ", value));

            // compile all parts from the Parts collection
            value = base.Compile();
            if (!string.IsNullOrEmpty(value))
                sb.Append(string.Format("{0} ", value));

            return sb.ToString().RemoveLineBreak();
        }

        #endregion

        public Func<string> Predicate { get; private set; }

        public override string ToString()
        {
            if (Predicate != null)
                return string.Format("Predicate: [{0}] Operation: [{1}]", Predicate.ToString(), OperationType.ToString());

            return string.Format("Predicate: [No predicate defined] Operation: [{0}]", OperationType.ToString());
        }
    }
}
