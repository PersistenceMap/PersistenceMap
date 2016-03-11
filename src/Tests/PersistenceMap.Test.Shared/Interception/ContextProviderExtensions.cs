namespace PersistenceMap.Interception
{
    public static class ContextProviderExtensions
    {
        public static DatabaseContext Open(this ContextProvider context)
        {
            return new DatabaseContext(context.ConnectionProvider, new Settings(), context.Interceptors);
        }
    }
}
