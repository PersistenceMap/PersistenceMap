using NUnit.Framework;
using System.Configuration;
using System.IO;

namespace PersistenceMap.SqlServer.Test
{
    //[SetUpFixture]
    public class AssemblyTestSetup
    {
        [SetUp]
        public void AssemblyInit()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PersistenceMap.Test.Properties.Settings.ConnectionString"].ConnectionString;
            var provider = new SqlContextProvider(connectionString);
            using (var ctx = provider.Open())
            {
                ctx.Database.Create();
                ctx.Commit();

                var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");
                string script = file.OpenText().ReadToEnd();
                ctx.Execute(script);
            }
        }

        [TearDown]
        public void AssemblyCleanup()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PersistenceMap.Test.Properties.Settings.ConnectionString"].ConnectionString;
            var provider = new SqlContextProvider(connectionString);
            using (var ctx = provider.Open())
            {
                ctx.Database.Drop();
                ctx.Commit();
            }
        }
    }
}
