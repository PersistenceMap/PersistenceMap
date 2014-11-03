
namespace PersistanceMap
{
    public class DatabaseConnection
    {
        private readonly IContextProvider _provider;

        public DatabaseConnection(IContextProvider provider)
        {
            _provider = provider;
            Settings = new Settings();
        }

        public virtual IDatabaseContext Open()
        {
            return new DatabaseContext(_provider, Settings.LoggerFactory);
        }

        public Settings Settings { get; private set; }
    }
}
