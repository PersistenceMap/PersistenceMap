using System;
using NUnit.Framework;
using PersistanceMap.Compiler;

namespace PersistanceMap.Test
{
    public delegate void ContextProviderCallbackHandler(string query);

    /// <summary>
    /// Represents a IContextProvider that only compares the generated sql string to a expected sql string withou executing to a database
    /// </summary>
    public class CallbackContextProvider : IContextProvider
    {
        //private readonly Action<string> _callback;
        public event ContextProviderCallbackHandler Callback;

        public CallbackContextProvider()
        {
        }

        public CallbackContextProvider(Action<string> callback)
        {
            Callback = (s) => callback(s);
        }

        public string ConnectionString { get; private set; }

        private IExpressionCompiler _expressionCompiler;

        public virtual IExpressionCompiler ExpressionCompiler
        {
            get
            {
                if (_expressionCompiler == null)
                    _expressionCompiler = new ExpressionCompiler();

                return _expressionCompiler;
            }
        }

        public IReaderContext Execute(string query)
        {
            return ExecuteNonQuery(query);
        }

        public IReaderContext ExecuteNonQuery(string query)
        {
            if(Callback == null)
                throw new ArgumentNullException("Callback was not set prior to execution");

            Callback(query);

            return null;
        }
    }
}
