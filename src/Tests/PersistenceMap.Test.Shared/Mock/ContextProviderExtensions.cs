namespace PersistenceMap.Mock
{
    public static class ContextProviderExtensions
    {
        public static PersistenceMap.DatabaseContext Open(this PersistenceMap.ContextProvider context)
        {
            return new PersistenceMap.DatabaseContext(context.ConnectionProvider, new Settings(), context.Interceptors);
        }
    }
}
