using System;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Object/expression container for expressions that get executed after the mapping
    /// </summary>
    public class CallbackMap
    {
        public CallbackMap(string id, Action<object> callback, Type callbackValueType)
        {
            Id = id;
            Callback = callback;
            CallbackValueType = callbackValueType;
        }

        public Action<object> Callback { get; private set; }

        public string Id { get; private set; }

        public Type CallbackValueType { get; private set; }
    }
}
