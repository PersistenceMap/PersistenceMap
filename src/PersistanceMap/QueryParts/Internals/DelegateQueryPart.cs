using System;
using System.Text;

namespace PersistanceMap.QueryParts
{
    public class DelegateQueryPart : ItemsQueryPart, IItemsQueryPart, IQueryPart
    {
        public DelegateQueryPart(OperationType operation, Func<string> callback)
            : this(operation, callback, operation.ToString())
        {
        }

        public DelegateQueryPart(OperationType operation, Func<string> callback, string id = null)
        {
            OperationType = operation;
            Delegate = callback;
            ID = id;
        }
        
        #region IQueryPart Implementation

        public override string Compile()
        {
            var sb = new StringBuilder();

            // compile the delegate
            sb.Append(string.Format("{0}", Delegate.Invoke() ?? ""));

            //// compile all parts from the Parts collection
            //sb.Append(string.Format("{0}", base.Compile() ?? ""));

            return sb.ToString().RemoveLineBreak();
        }

        #endregion

        public string ID { get; set; }

        public Func<string> Delegate { get; private set; }
        
        public override string ToString()
        {
            if (Delegate != null)
                return string.Format("{0} - Operation: [{1}] Delegate: [{2}]", GetType().Name, OperationType.ToString(), Delegate.ToString());

            return string.Format("{0} - Operation: [{1}] Delegate: [No delegate defined]", GetType().Name, OperationType.ToString());
        }
    }
}
