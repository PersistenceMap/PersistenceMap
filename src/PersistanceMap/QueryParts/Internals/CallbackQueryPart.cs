using System;

namespace PersistanceMap.QueryParts
{
    internal class CallbackQueryPart : IQueryPart
    {
        readonly Func<string> _callback;

        public CallbackQueryPart(OperationType operationtype, Func<string> callback, string id = null)
        {
            OperationType = operationtype;
            _callback = callback;
            ID = id;
        }

        public string ID { get; set; }

        public OperationType OperationType { get; set; }

        public string Compile()
        {
            return _callback.Invoke();
        }
    }
}
