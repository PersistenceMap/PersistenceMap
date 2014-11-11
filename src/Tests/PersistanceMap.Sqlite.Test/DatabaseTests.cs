using NUnit.Framework;
using PersistanceMap.Sqlite.Test.TableTypes;
using System;
using System.IO;

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
            }
        }

        [Test]
        public void AlterTableDropForeignKey()
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

                context.Database.Table<Warrior>()
                    .DropKey(wrir => wrir.WeaponID)
                    .Alter();

                context.Commit();
            }
        }

        [Test]
        public void AlterTable()
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

                context.Database.Table<Warrior>()
                    .Field(wrir => wrir.Race, FieldOperation.Add)
                    .Alter();

                context.Commit();
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

                // drop the table
                context.Database.Table<Weapon>()
                    .Drop();

                context.Commit();
            }
        }
    }
}
