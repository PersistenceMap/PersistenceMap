using System;
using System.Data;

namespace PersistanceMap.QueryBuilder
{
    public interface ICallbackHandlerQueryPart
    {
        bool CanHandleCallback { get; }

        void HandleCallback(IDataReader reader);
    }

    public interface ICallbackQueryPart<T> : ICallbackHandlerQueryPart, IQueryPart
    {
        Action<T> Callback { get; set; }
    }
}
