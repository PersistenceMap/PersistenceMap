using System;
using System.Data;

namespace PersistanceMap.QueryBuilder
{
    public interface ICallbackHandlerQueryPart
    {
        /// <summary>
        /// Gets the name of the output parameter
        /// </summary>
        string CallbackName { get; set; }

        /// <summary>
        /// Gets the type of the output parameter
        /// </summary>
        Type CallbackType { get; }

        /// <summary>
        /// Gets if the instance containes a registered callback
        /// </summary>
        bool CanHandleCallback { get; }

        //string CompileOutParameter(int index);

        /// <summary>
        /// Try to handle the callback
        /// </summary>
        /// <param name="value"></param>
        bool TryHandleCallback(object value);
    }

    public interface ICallbackQueryPart<T> : ICallbackHandlerQueryPart, IQueryPart
    {
        /// <summary>
        /// The callback
        /// </summary>
        Action<T> Callback { get; set; }
    }
}
