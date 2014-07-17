using System;
using System.Data;

namespace PersistanceMap.QueryBuilder
{
    public interface ICallbackHandlerQueryPart
    {
        string CallbackParameterName { get; }

        bool CanHandleCallback { get; }

        string CompileOutParameter(int index);

        void HandleCallback(IDataReader reader);
    }

    public interface ICallbackQueryPart<T> : ICallbackHandlerQueryPart, IQueryPart
    {
        Action<T> Callback { get; set; }
    }
}
