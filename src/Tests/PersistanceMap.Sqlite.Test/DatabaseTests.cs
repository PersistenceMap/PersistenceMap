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

        [Test]
        public void CreateDatabase()
        {
            Assert.IsFalse(File.Exists(DatabaseName));

            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                Assert.IsFalse(File.Exists(DatabaseName));

                context.Commit();

                Assert.IsTrue(File.Exists(DatabaseName));
            }
        }

        [Test]
        public void CreateTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                // table with multiple columns for key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .Create();

                context.Commit();
            }
        }

        [Test]
        public void CreateTableMultyKey()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                // table with multiple columns for key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID, wrir => wrir.WeaponID)
                    .Create();

                context.Commit();
            }
        }

        [Test]
        public void CreateTableForeignKey()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.WeaponID)
                    .Create();

                // table with a foreign key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .Key<Weapon>(wrir => wrir.WeaponID, wpn => wpn.WeaponID)
                    .Create();

                context.Commit();
            }
        }

        [Test]
        public void AlterTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.WeaponID)
                    .Create();

                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .Key<Weapon>(wrir => wrir.WeaponID, wpn => wpn.WeaponID)
                    .Create();

                Assert.Ignore();

                context.Commit();
            }
        }

        [Test]
        public void DropTable()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Create();

                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.WeaponID)
                    .Create();

                context.Commit();

                // drop a table
                context.Database.Table<Weapon>()
                    .Drop();

                context.Commit();
            }
        }
    }
}
