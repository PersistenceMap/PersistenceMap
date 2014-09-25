using System;

namespace PersistanceMap.QueryParts
{
    internal class CallbackQueryPart : IQueryPart
    {
        readonly Func<string> _callback;

        public CallbackQueryPart(OperationType operationtype, Func<string> callback)
        {
            OperationType = operationtype;
            _callback = callback;
        }

        public OperationType OperationType { get; set; }

        public string Compile()
        {
            return _callback.Invoke();
        }
    }
}
