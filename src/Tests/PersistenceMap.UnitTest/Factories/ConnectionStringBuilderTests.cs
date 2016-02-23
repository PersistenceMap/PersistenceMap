using NUnit.Framework;

namespace PersistenceMap.UnitTest.Factories
{
    [TestFixture]
    public class ConnectionStringBuilderTests
    {
        [Test]
        public void PersistenceMap_ConnectionStringBuilder_GetDatabaseFromInitialCatalogTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "data source=.;Initial Catalog =WarriorDB;persist security info=False;user id=sa";
            var database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;Initial Catalog=WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;initial catalog =WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;initial catalog=WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;initial catalog= WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_GetDatabaseFromDatabaseTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "data source=.;Database =WarriorDB;persist security info=False;user id=sa";
            var database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;Database=WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;database=WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;database =WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");

            connectionString = "data source=.;database= WarriorDB;persist security info=False;user id=sa";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_GetDatabaseFromDataSourceTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "Data Source =WarriorDB.db";
            var database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = "Data Source=WarriorDB.db";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = "data source =WarriorDB.db";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = "data source=WarriorDB.db";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = "data source= WarriorDB.db";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = "data source=WarriorDB.db;";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, "WarriorDB.db");

            connectionString = @"Data Source=c:\WarriorDB.db;Version=3;New=True;";
            database = builder.GetDatabase(connectionString);
            Assert.AreEqual(database, @"c:\WarriorDB.db");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_SetDatabaseFromInitialCatalogTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "data source=.;Initial Catalog =TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;Initial Catalog=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;Initial Catalog=TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;Initial Catalog=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;initial catalog=TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;initial catalog=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;initial catalog=TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;initial catalog=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;initial catalog= TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;initial catalog=WarriorDB;persist security info=False;user id=sa");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_SetDatabaseFromDatabaseTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "data source=.;Database =TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;Database=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;Database=TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;Database=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;database=TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;database=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;database =TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;database=WarriorDB;persist security info=False;user id=sa");

            connectionString = "data source=.;database= TempDB;persist security info=False;user id=sa";
            connectionString = builder.SetDatabase("WarriorDB", connectionString);
            Assert.AreEqual(connectionString, "data source=.;database=WarriorDB;persist security info=False;user id=sa");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_SetDatabaseFromDataSourceTest()
        {
            var builder = new ConnectionStringBuilder();

            var connectionString = "Data Source =TempDB.db";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "Data Source=WarriorDB.db");

            connectionString = "Data Source=TempDB.db";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "Data Source=WarriorDB.db");

            connectionString = "data source =TempDB.db";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "data source=WarriorDB.db");

            connectionString = "data source=TempDB.db";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "data source=WarriorDB.db");

            connectionString = "data source= TempDB.db";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "data source=WarriorDB.db");

            connectionString = "data source=TempDB.db;";
            connectionString = builder.SetDatabase("WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, "data source=WarriorDB.db;");

            connectionString = @"Data Source=c:\TempDB.db;Version=3;New=True;";
            connectionString = builder.SetDatabase(@"c:\WarriorDB.db", connectionString);
            Assert.AreEqual(connectionString, @"Data Source=c:\WarriorDB.db;Version=3;New=True;");
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_FormatConnectionString_LocalDB()
        {
            var builder = new ConnectionStringBuilder();
            var connectionString = @"Data Source=(LocalDB)\mssqllocaldb;AttachDBFileName=c:\Path\To\Database\WickeFlame.mdf;Initial Catalog=WickeFlame;Integrated Security=True;";

            Assert.AreEqual(builder.FormatConnectionString(connectionString), connectionString);
        }

        [Test]
        public void PersistenceMap_ConnectionStringBuilder_FormatConnectionString_MasterForLocalDB()
        {
            var builder = new ConnectionStringBuilder();
            var connectionString = @"Data Source=(LocalDB)\mssqllocaldb;AttachDBFileName=c:\Path\To\Database\WickeFlame.mdf;Initial Catalog=WickeFlame;Integrated Security=True;";

            connectionString = builder.SetDatabase("Master", connectionString);
            var master = builder.FormatConnectionString(connectionString);

            Assert.AreEqual(master, @"Data Source=(LocalDB)\mssqllocaldb;Initial Catalog=Master;Integrated Security=True;");
        }
    }
}
