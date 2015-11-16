namespace PersistenceMap.Mock
{
    public class DataReaderContext : IDataReaderContext
    {
        public System.Data.IDataReader DataReader
        {
            get
            {
                return new DataReader();
            }
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}
