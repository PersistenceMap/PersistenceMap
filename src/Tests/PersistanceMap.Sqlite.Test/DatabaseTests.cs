using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.IO;
using System.Linq;

namespace PersistanceMap.Sqlite.Test
{
    [TestFixture]
    public class DatabaseTests : TestBase
    {
        const string DatabaseName = "SQLiteDemo.db";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(DatabaseName))
                File.Delete(DatabaseName);
        }

        //[Test]
        //public void CreateDatabase()
        //{
        //    Assert.IsFalse(File.Exists(DatabaseName));

        //    var provider = new SqliteContextProvider(ConnectionString);
        //    using (var context = provider.Open())
        //    {
        //        context.Database.Create();

        //        Assert.IsFalse(File.Exists(DatabaseName));

        //        context.Commit();

        //        Assert.IsTrue(File.Exists(DatabaseName));
        //    }
        //}

        [Test]
        public void CreateTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // table with multiple columns for key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .Create();

                Assert.IsFalse(File.Exists(DatabaseName));

                context.Commit();

                Assert.IsTrue(File.Exists(DatabaseName));
            }
        }

        [Test]
        public void CreateTableMultyKey()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // table with multiple columns for key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID, wrir => wrir.WeaponID)
                    .Create();

                Assert.IsFalse(File.Exists(DatabaseName));

                context.Commit();

                Assert.IsTrue(File.Exists(DatabaseName));
            }
        }

        [Test]
        public void CreateTableForeignKey()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.ID)
                    .Create();

                // table with a foreign key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID)
                    .Create();

                Assert.IsFalse(File.Exists(DatabaseName));

                context.Commit();

                Assert.IsTrue(File.Exists(DatabaseName));

                var tables = context.Select<Sqlite_Master>(m => m.Type == "table");
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        //[Test]
        //public void AlterTableDropForeignKey()
        //{
        //    var provider = new SqliteContextProvider(ConnectionString);
        //    using (var context = provider.Open())
        //    {
        //        context.Database.Table<Weapon>()
        //            .Key(wpn => wpn.ID)
        //            .Create();

        //        // table with a foreign key
        //        context.Database.Table<Warrior>()
        //            .Key(wrir => wrir.ID)
        //            .ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID)
        //            .Create();

        //        Assert.IsFalse(File.Exists(DatabaseName));

        //        context.Commit();

        //        Assert.IsTrue(File.Exists(DatabaseName));

        //        context.Database.Table<Warrior>()
        //            .DropKey(wrir => wrir.WeaponID)
        //            .Alter();

        //        context.Commit();

        //        var tables = context.Select<Sqlite_Master>(m => m.Type == "table");
        //        Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
        //        Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
        //    }
        //}

        [Test]
        public void AlterTableAddField()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.ID)
                    .Create();

                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID)
                    .Ignore(wrir => wrir.Race)
                    .Create();

                context.Commit();

                //TODO: Check table definition

                context.Database.Table<Warrior>()
                    .Column(wrir => wrir.Race, FieldOperation.Add)
                    .Alter();

                context.Commit();

                //TODO: Check table definition

                var tables = context.Select<Sqlite_Master>(m => m.Type == "table");
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        [Test]
        public void DropTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // create a table to drop later in the test
                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.ID)
                    .Create();

                context.Commit();

                var tables = context.Select<Sqlite_Master>(m => m.Type == "table");
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));

                // drop the table
                context.Database.Table<Weapon>()
                    .Drop();

                context.Commit();

                tables = context.Select<Sqlite_Master>(m => m.Type == "table");
                Assert.IsFalse(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        [Test]
        public void RenameTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                // create a table to drop later in the test
                context.Database.Table<Warrior>().Create();

                context.Commit();

                // drop the table
                context.Database.Table<Warrior>().RenameTo<Solidier>();

                context.Commit();

                var tables = context.Select<Sqlite_Master>(m => m.Type == "table");
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Solidier).Name));
                Assert.IsFalse(tables.Any(t => t.Name == typeof(Warrior).Name));
            }
        }
    }
}
