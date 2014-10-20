
namespace PersistanceMap
{
    public class DatabaseConnection
    {
        private readonly IContextProvider _provider;

        public DatabaseConnection(IContextProvider provider)
        {
            _provider = provider;
        }

        public virtual IDatabaseContext Open()
        {
            return new DatabaseContext(_provider);
        }
    }
}
