using System.Configuration;

namespace PersistenceMap.Sqlite.Test
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
    }
}
