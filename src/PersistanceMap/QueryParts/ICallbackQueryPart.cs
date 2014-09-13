using System;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Callback implementation had to be placed in two interfaces to prevent the base interface to be generic. 
    /// This interface is placed into a collection witch doesn't allow generic type refernce because each entry has its own type.
    /// </summary>
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
