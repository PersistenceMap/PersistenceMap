﻿using NUnit.Framework;
using PersistanceMap.Test;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace PersistanceMap.SqlServer.Test
{
    [TestFixture]
    public class DatabaseTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            var database = provider.ConnectionProvider.Database;
            provider.ConnectionProvider.Database = "Master";
            using (var context = provider.Open())
            {
                try
                {
                    if (context.Execute(string.Format("SELECT * FROM Master.sys.databases WHERE Name = '{0}'", database), () => new { Name = "" }).Any())
                    {
                        //context.Execute(string.Format("DROP DATABASE {0}", database));
                        provider.ConnectionProvider.Database = database;

                        var tables = GetTables(context);
                        if (tables.Any(t => t.Name == typeof(Warrior).Name))
                            context.Database.Table<Warrior>().Drop();

                        if (tables.Any(t => t.Name == typeof(Weapon).Name))
                            context.Database.Table<Weapon>().Drop();
                    }
                }
                catch (SqlException) { }
            }
        }

        private void CreateDatabaseIfNotExists()
        {
            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            var database = provider.ConnectionProvider.Database;
            provider.ConnectionProvider.Database = "Master";
            using (var context = provider.Open())
            {
                try
                {
                    if (context.Execute(string.Format("SELECT * FROM Master.sys.databases WHERE Name = '{0}'", database), () => new { Name = "" }).Any() == false)
                    {
                        provider.ConnectionProvider.Database = database;
                        context.Database.Create();

                        context.Commit();
                    }
                }
                catch (SqlException) { }
            }
        }

        public void DropDatabaseIfExists()
        {
            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            var database = provider.ConnectionProvider.Database;
            provider.ConnectionProvider.Database = "Master";
            using (var context = provider.Open())
            {
                try
                {
                    if (context.Execute(string.Format("SELECT * FROM Master.sys.databases WHERE Name = '{0}'", database), () => new { Name = "" }).Any())
                    {
                        context.Execute(string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", database));
                        context.Execute(string.Format("DROP DATABASE {0}", database));
                    }
                }
                catch (SqlException e) 
                {
                    System.Diagnostics.Trace.WriteLine(e);
                }
            }
        }

        private IEnumerable<Sysobjects> GetTables(SqlDatabaseContext context)
        {
            return context.Select<Sysobjects>(so => so.Type == "U");
        }

        [Test]
        public void CreateDatabase()
        {
            DropDatabaseIfExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            using (var context = provider.Open())
            {
                context.Database.Create();

                context.Commit();

                var databases = context.Execute(string.Format("SELECT * FROM Master.sys.databases WHERE Name = '{0}'", provider.ConnectionProvider.Database), () => new { Name = "" });
                Assert.IsTrue(databases.Any(db => db.Name == provider.ConnectionProvider.Database));
            }
        }

        [Test]
        public void CreateDatabaseWithTable()
        {
            DropDatabaseIfExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            using (var context = provider.Open())
            {
                context.Database.Create();
                context.Database.Table<Warrior>().Create();
                context.Insert(() => new Warrior
                {
                    ID = 1,
                    Race ="Elf"
                });

                context.Commit();

                var databases = context.Execute(string.Format("SELECT * FROM Master.sys.databases WHERE Name = '{0}'", provider.ConnectionProvider.Database), () => new { Name = "" });
                Assert.IsTrue(databases.Any(db => db.Name == provider.ConnectionProvider.Database));

                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));

                var warriors = context.Select<Warrior>(wrir => wrir.ID == 1);
                Assert.IsTrue(warriors.Any());
                Assert.IsTrue(warriors.First().Race == "Elf");
            }
        }
        
        [Test]
        public void CreateTableMultyKey()
        {
            CreateDatabaseIfNotExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            using (var context = provider.Open())
            {
                // table with multiple columns for key
                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID, wrir => wrir.WeaponID)
                    .Create();

                context.Commit();

                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
            }
        }

        [Test]
        public void CreateTableForeignKey()
        {
            CreateDatabaseIfNotExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
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
                
                context.Commit();

                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }
        
        [Test]
        public void AlterTableAddColumn()
        {
            CreateDatabaseIfNotExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
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
                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        [Test]
        public void AlterTableDropColumn()
        {
            CreateDatabaseIfNotExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            using (var context = provider.Open())
            {
                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.ID)
                    .Create();

                context.Database.Table<Warrior>()
                    .Key(wrir => wrir.ID)
                    .ForeignKey<Weapon>(wrir => wrir.WeaponID, wpn => wpn.ID)
                    .Create();

                context.Commit();

                //TODO: Check table definition

                context.Database.Table<Warrior>()
                    .Column(wrir => wrir.Race, FieldOperation.Drop)
                    .Alter();

                context.Commit();

                //TODO: Check table definition
                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Warrior).Name));
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        [Test]
        public void DropTable()
        {
            CreateDatabaseIfNotExists();

            var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
            using (var context = provider.Open())
            {
                // create a table to drop later in the test
                context.Database.Table<Weapon>()
                    .Key(wpn => wpn.ID)
                    .Create();

                context.Commit();

                var tables = GetTables(context);
                Assert.IsTrue(tables.Any(t => t.Name == typeof(Weapon).Name));

                // drop the table
                context.Database.Table<Weapon>()
                    .Drop();

                context.Commit();

                tables = GetTables(context);
                Assert.IsFalse(tables.Any(t => t.Name == typeof(Weapon).Name));
            }
        }

        //[Test]
        //public void RenameTable()
        //{
        //    var provider = new SqlContextProvider(GetConnectionString("WarriorDB"));
        //    using (var context = provider.Open())
        //    {
        //        context.Database.Create();

        //        // create a table to drop later in the test
        //        context.Database.Table<Warrior>().Create();

        //        context.Commit();

        //        // drop the table
        //        context.Database.Table<Warrior>().RenameTo<Solidier>();

        //        context.Commit();

        //        var tables = GetTables(context);
        //        Assert.IsTrue(tables.Any(t => t.Name == typeof(Solidier).Name));
        //        Assert.IsFalse(tables.Any(t => t.Name == typeof(Warrior).Name));
        //    }
        //}
    }
}