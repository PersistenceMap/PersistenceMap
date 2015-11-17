using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceMap.SqlServer.Test
{
    [TestFixture]
    [NUnit.Framework.Ignore("Logic not yet implemented")]
    public class SqlServerCreateLocalDbTests
    {
        [Test]
        public void SqlServer_CreateLocalDb_Test()
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

            var connectionString = string.Format(@"Data Source=(LocalDB)\mssqllocaldb;AttachDBFileName={0};Initial Catalog={1};Integrated Security=True;", databaseMdfPath, databaseMdfPath);

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }
        }
    }
}
