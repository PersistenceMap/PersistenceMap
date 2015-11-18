using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    public class SqlServerCreateDbTests
    {
        [Test]
        [NUnit.Framework.Ignore("Connection is not released")]
        public void SqlServer_CreateLocalDb_Test()
        {
            var databaseName = "WarriorDB";
                        
            var connectionString = string.Format(@"Data Source=(LocalDB)\mssqllocaldb;Initial Catalog={0};Integrated Security=True;", databaseName);

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            Assert.Fail("The connection is not released b the first connection so it can't be deleted");
            //SELECT DB_NAME(dbid) as DBName, COUNT(dbid) as NumberOfConnections, loginame as LoginName FROM sys.sysprocesses WHERE dbid > 0 GROUP BY dbid, loginame
            //SELECT loginame as LoginName, *FROM sys.sysprocesses WHERE dbid > 0
            //exec sp_who
            //exec sp_who2
            using (var context = provider.Open())
            {
                context.Database.Drop();
                context.Commit();
            }
        }

        [Test]
        //[NUnit.Framework.Ignore("Connection is not released")]
        public void SqlServer_CreateLocalDbWithDbFilePath_Test()
        {
            var databaseName = "WarriorDB";
            var outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
            var mdfFilename = string.Format("{0}.mdf", databaseName);
            var databaseMdfPath = Path.Combine(outputFolder, mdfFilename);

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            var connectionString = string.Format(@"Data Source=(LocalDB)\mssqllocaldb;AttachDBFileName={0};Initial Catalog={1};Integrated Security=True;", databaseMdfPath, databaseName);

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            Assert.Fail("The connection is not released b the first connection so it can't be deleted");
            using (var context = provider.Open())
            {
                context.Database.Drop();
                context.Commit();
            }
        }
    }
}
