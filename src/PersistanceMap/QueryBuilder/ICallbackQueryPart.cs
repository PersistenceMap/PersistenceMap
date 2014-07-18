using System;
using System.Data;

namespace PersistanceMap.QueryBuilder
{
    public interface ICallbackHandlerQueryPart
    {
        string CallbackParameterName { get; }

        Type CallbackParameterType { get; }

        bool CanHandleCallback { get; }

        string CompileOutParameter(int index);

        void TryHandleCallback(object value); //IDataReader reader);
    }

    public interface ICallbackQueryPart<T> : ICallbackHandlerQueryPart, IQueryPart
    {
        Action<T> Callback { get; set; }
    }
}
