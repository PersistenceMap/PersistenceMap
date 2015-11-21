using System;

namespace PersistenceMap.QueryParts
{
    /// <summary>
    /// Object/expression container for expressions that is executed after the mapping of the data
    /// </summary>
    public class AfterMapCallbackPart
    {
        public AfterMapCallbackPart(string id, Action<object> callback, Type callbackValueType)
        {
            Id = id;
            Callback = callback;
            CallbackValueType = callbackValueType;
        }

        /// <summary>
        /// The delegate that is executed after the mapping of the Data
        /// </summary>
        public Action<object> Callback { get; private set; }

        /// <summary>
        /// The id of the delegate
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The type of the value that is passed to the delegate
        /// </summary>
        public Type CallbackValueType { get; private set; }
    }
}
