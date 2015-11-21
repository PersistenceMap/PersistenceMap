using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Linq;
using System;

namespace PersistenceMap.SqlServer.Test.Integration
{
    [TestFixture]
    public class SqlServerCreateDbTests
    {
        [Test]
        public void SqlServer_LocalDb_CreateLocalDb_Test()
        {
            var databaseName = "WarriorDB";

            var connectionString = $"Data Source=(LocalDB)\\mssqllocaldb;Initial Catalog={databaseName};Integrated Security=True;";

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            using (var context = provider.Open())
            {
                var database = context.Execute($"SELECT * FROM Master.sys.databases WHERE Name = '{databaseName}'", () => new { Name = "" });
                Assert.IsTrue(database.Any());
            }

            RemoveDatabase(provider);
        }

        [Test]
        public void SqlServer_LocalDb_CreateLocalDbWithDbFilePath_Test()
        {
            var databaseName = "WarriorDB";
            var outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
            var mdfFilename = $"{databaseName}.mdf";
            var databaseMdfPath = Path.Combine(outputFolder, mdfFilename);

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            var connectionString = $"Data Source=(LocalDB)\\mssqllocaldb;AttachDBFileName={databaseMdfPath};Initial Catalog={databaseName};Integrated Security=True;";

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            using (var context = provider.Open())
            {
                var database = context.Execute($"SELECT * FROM Master.sys.databases WHERE Name = '{databaseName}'", () => new { Name = "" });
                Assert.IsTrue(database.Any());
            }

            RemoveDatabase(provider);
        }

        [Test]
        public void SqlServer_LocalDb_MultipleConnections_Test()
        {
            var databaseName = "WarriorDB";

            var connectionString = $"Data Source=(LocalDB)\\mssqllocaldb;Initial Catalog={databaseName};Integrated Security=True;";

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Item>().Create();
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Insert<Item>(() => new { ID = 1, Name = "One" });
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Insert<Item>(() => new { ID = 2, Name = "Two" });
                context.Commit();
            }

            using (var context = provider.Open())
            {
                var connections = context.Execute("SELECT DB_NAME(dbid) as DBName, COUNT(dbid) as NumberOfConnections, loginame as LoginName FROM sys.sysprocesses WHERE dbid > 0 GROUP BY dbid, loginame", () => new
                {
                    DBName = "",
                    NumberOfConnections = 0,
                    LoginName = ""
                });

                Assert.IsTrue(connections.Where(c => c.DBName == databaseName).Count() == 1);
                Assert.IsTrue(connections.FirstOrDefault(c => c.DBName == databaseName).NumberOfConnections == 1);
            }

            RemoveDatabase(provider);
        }

        [Test]
        public void SqlServer_CreateDb_Test()
        {
            var databaseName = "WarriorDB1";

            var connectionString = $"Data Source=.;Initial Catalog={databaseName};Integrated Security=True;";

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            using (var context = provider.Open())
            {
                var database = context.Execute($"SELECT * FROM Master.sys.databases WHERE Name = '{databaseName}'", () => new { Name = "" });
                Assert.IsTrue(database.Any());
            }

            RemoveDatabase(provider);
        }

        [Test]
        public void SqlServer_MultipleConnections_Test()
        {
            var databaseName = "WarriorDB2";

            var connectionString = $"Data Source=.;Initial Catalog={databaseName};Integrated Security=True;";

            var provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Item>().Create();
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Insert<Item>(() => new { ID = 1, Name = "One" });
                context.Commit();
            }

            provider = new SqlContextProvider(connectionString);
            using (var context = provider.Open())
            {
                context.Insert<Item>(() => new { ID = 2, Name = "Two" });
                context.Commit();
            }

            using (var context = provider.Open())
            {
                var connections = context.Execute("SELECT DB_NAME(dbid) as DBName, COUNT(dbid) as NumberOfConnections, loginame as LoginName FROM sys.sysprocesses WHERE dbid > 0 GROUP BY dbid, loginame", () => new
                {
                    DBName = "",
                    NumberOfConnections = 0,
                    LoginName = ""
                });

                Assert.IsTrue(connections.Where(c => c.DBName == databaseName).Count() == 1);
                Assert.IsTrue(connections.FirstOrDefault(c => c.DBName == databaseName).NumberOfConnections == 1);
            }

            RemoveDatabase(provider);
        }

        private void RemoveDatabase(SqlContextProvider provider)
        {
            // SELECT DB_NAME(dbid) as DBName, COUNT(dbid) as NumberOfConnections, loginame as LoginName FROM sys.sysprocesses WHERE dbid > 0 GROUP BY dbid, loginame
            // SELECT loginame as LoginName, *FROM sys.sysprocesses WHERE dbid > 0
            // exec sp_who
            // exec sp_who2
            using (var context = provider.Open())
            {
                var dbFiles = context.Execute("SELECT * FROM sys.database_files", () => new { Name = "", Physical_Name = "" });

                context.Database.Detach();
                context.Commit();

                foreach (var file in dbFiles)
                {
                    File.Delete(file.Physical_Name);
                }
            }
        }

        public class Item
        {
            public int ID { get; set; }

            public string Name { get; set; }
        }
    }
}
