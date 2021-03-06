﻿using NUnit.Framework;
using PersistenceMap.Test;
using PersistenceMap.Test.TableTypes;
using System;
using System.IO;
using System.Linq;

namespace PersistenceMap.Sqlite.Test
{
    [TestFixture]
    public class DatabaseTests : TestBase
    {
        private const string DatabaseName = "SQLiteDemo.db";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(DatabaseName))
            {
                File.Delete(DatabaseName);
            }
        }
        
        [Test]
        public void PersistenceMap_Sqlite_Integration_CreateTable()
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
        public void PersistenceMap_Sqlite_Integration_GetDatabaseFromConnectionStringTest()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                Assert.AreEqual($".\\{DatabaseName}", context.ConnectionProvider.Database);

                context.ConnectionProvider.Database = "Test.db";
                Assert.AreEqual(context.ConnectionProvider.Database, "Test.db");
            }
        }

        [Test]
        public void PersistenceMap_Sqlite_Integration_CreateTableMultyKey()
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
        public void PersistenceMap_Sqlite_Integration_CreateTableForeignKey()
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

        [Test]
        public void PersistenceMap_Sqlite_Integration_CreateTableNotNullableColumn()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            var logger = new MessageStackLogWriter();
            provider.Settings.AddLogWriter(logger);
            provider.Settings.LogLevel = LogDebth.Simple;

            using (var context = provider.Open())
            {
                // table with a foreign key
                context.Database.Table<Warrior>()
                    .Column(wrir => wrir.ID)
                    .Column(wrir => wrir.Race, isNullable: false)
                    .Create();

                Assert.IsFalse(File.Exists(DatabaseName));
                
                context.Commit();

                Assert.AreEqual(logger.Logs.First().Message.Flatten(), "CREATE TABLE IF NOT EXISTS Warrior (ID int NOT NULL, Race varchar(1000) NOT NULL, Name varchar(1000), WeaponID int NOT NULL, SpecialSkill varchar(1000))");
            }
        }

        [Test]
        public void PersistenceMap_Sqlite_Integration_AlterTableAddField()
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
        public void PersistenceMap_Sqlite_Integration_DropTable()
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
        public void PersistenceMap_Sqlite_Integration_RenameTable()
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

        [Test]
        public void PersistenceMap_Sqlite_Integration_AddFieldByString()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            var logger = new MessageStackLogWriter();
            provider.Settings.AddLogWriter(logger);
            using (var context = provider.Open())
            {
                context.Database.Table<Warrior>().Ignore(wrir => wrir.Race).Create();
                context.Database.Table<Warrior>().Column("Race", FieldOperation.Add, typeof(string)).Alter();
                context.Commit();
            }
        }

        [Test]
        public void PersistenceMap_Sqlite_Integration_AddFieldByStringFail()
        {
            var provider = new SqliteContextProvider(ConnectionString);
            using (var context = provider.Open())
            {
                context.Database.Table<Warrior>().Ignore(wrir => wrir.Race).Create();
                Assert.Throws<ArgumentNullException>(() => context.Database.Table<Warrior>().Column("Race", FieldOperation.Add).Alter());
            }
        }
    }
}
