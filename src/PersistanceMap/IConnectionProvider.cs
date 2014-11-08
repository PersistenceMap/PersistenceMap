using System;

namespace PersistanceMap
{
    public interface IConnectionProvider : IDisposable
    {
        IQueryCompiler QueryCompiler { get; }

        IReaderContext Execute(string query);

        IReaderContext ExecuteNonQuery(string query);
    }
}
