using System.Configuration;

namespace PersistanceMap.Sqlite.Test
{
    public abstract class TestBase
    {
        protected string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["PersistanceMap.Test.Properties.Settings.ConnectionString"].ConnectionString;
            }
        }
    }
}
