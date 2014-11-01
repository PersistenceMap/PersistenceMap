
namespace PersistanceMap
{
    public class DatabaseConnection
    {
        private readonly IContextProvider _provider;

        public DatabaseConnection(IContextProvider provider)
        {
            _provider = provider;
            Options = new DatabaseOptions();
        }

        public virtual IDatabaseContext Open()
        {
            return new DatabaseContext(_provider, Options.LoggerFactory);
        }

        public DatabaseOptions Options { get; private set; }
    }
}
