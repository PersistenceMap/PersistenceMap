using System.Configuration;

namespace PersistenceMap.Test
{
    public abstract class TestBase
    {
        protected string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["PersistenceMap.Test.Properties.Settings.ConnectionString"].ConnectionString;
            }
        }

        protected string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
