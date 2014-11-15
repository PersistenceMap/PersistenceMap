using System.Configuration;

namespace PersistanceMap.Test
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
