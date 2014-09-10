using System;
using System.Data;

namespace PersistanceMap
{
    public interface IReaderContext : IDisposable
    {
        IDataReader DataReader { get; }

        //void Open();

        void Close();
    }
}
