
namespace PersistanceMap
{
    public class DatabaseConnection
    {
        private readonly IContextProvider _provider;

        public DatabaseConnection(IContextProvider provider)
        {
            _provider = provider;
            Options = new Settings();
        }

        public virtual IDatabaseContext Open()
        {
            return new DatabaseContext(_provider, Options.LoggerFactory);
        }

        public Settings Options { get; private set; }
    }
}
