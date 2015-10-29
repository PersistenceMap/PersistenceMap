using System;

namespace PersistanceMap.QueryParts
{
    public class DelegateQueryPart : ItemsQueryPart, IItemsQueryPart, IQueryPart
    {
        public DelegateQueryPart(OperationType operation, Func<string> callback)
            : this(operation, callback, null, null)
        {
        }

        public DelegateQueryPart(OperationType operation, Func<string> callback, Type entityType, string id = null) 
            : base(operation, entityType, id)
        {
            Delegate = callback;
        }

        public Func<string> Delegate { get; private set; }

        #region IQueryPart Implementation

        public override string Compile()
        {
            // compile the delegate
            var value = string.Format("{0}", Delegate.Invoke() ?? string.Empty);

            return value.RemoveLineBreak();
        }

        #endregion
        
        public override string ToString()
        {
            if (Delegate != null)
            {
                return string.Format("{0} - Operation: [{1}] Delegate: [{2}]", GetType().Name, OperationType.ToString(), Delegate.ToString());
            }

            return string.Format("{0} - Operation: [{1}] Delegate: [No delegate defined]", GetType().Name, OperationType.ToString());
        }
    }
}
