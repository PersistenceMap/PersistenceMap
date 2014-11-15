using System;

namespace PersistanceMap
{
    public interface IConnectionProvider : IDisposable
    {
        //string ConnectionString { get; }

        string Database { get; set; }

        IQueryCompiler QueryCompiler { get; }

        IReaderContext Execute(string query);

        IReaderContext ExecuteNonQuery(string query);
    }
}
