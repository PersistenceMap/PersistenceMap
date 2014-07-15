using System;

namespace PersistanceMap
{
    public interface IContextProvider
    {
        string ConnectionString { get; }

        IExpressionCompiler ExpressionCompiler { get; }

        IReaderContext Execute(string query);

        IReaderContext ExecuteNonQuery(string query);
    }
}
