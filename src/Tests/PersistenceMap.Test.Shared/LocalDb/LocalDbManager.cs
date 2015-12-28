using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PersistenceMap.Test.LocalDb
{
    public class LocalDbManager : IDisposable
    {
        static LocalDbManager()
        {
            DatabaseDirectory = "Data";
        }

        public LocalDbManager()
            : this(null)
        {
        }

        public LocalDbManager(string databaseName)
        {
            DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? "Database_" + Guid.NewGuid().ToString().Normalize().Replace("-", string.Empty) : databaseName;

            try
            {
                CreateDatabase();
                Trace.WriteLine($"DatabaseManager created the Local Database {Path.Combine(DatabaseDirectory, DatabaseName)}");
            }
            catch (SqlException e)
            {
                Trace.WriteLine($"DatabaseManager could not create Database {DatabaseName}.\\n\\r{e.Message}");
                throw new Exception($"LocalDbManager could not Create the Database {DatabaseName}", e);
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~LocalDbManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public static string DatabaseDirectory { get; set; }

        public string ConnectionString { get; private set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        // ReSharper disable once MemberCanBePrivate.Global
        public string DatabaseName { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string OutputFolder { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string DatabaseMdfPath { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string DatabaseLogPath { get; private set; }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    DetachDatabase(false);

                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public void ExecuteString(string script)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                ExecuteNonQuery(connection, script);
                connection.Close();
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private int ExecuteNonQuery(SqlConnection connection, string query)
        {
            query = RemoveCommentsFromQuery(query);

            // SqlCommand can't handle go breakes so split all go
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(query);

            var transaction = connection.BeginTransaction();
            var affectedRows = 0;
            using (var command = connection.CreateCommand())
            {
                try
                {
                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            command.CommandText = line;
                            command.Transaction = transaction;

                            affectedRows = command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    Trace.WriteLine(e);

                    throw new Exception("LocalDbManager caused an exception when calling ExecuteNonQuery", e);
                }
            }

            transaction.Commit();

            return affectedRows;
        }

        private static string RemoveCommentsFromQuery(string query)
        {
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(query, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                    {
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    }

                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);

            return noComments;
        }

        private void CreateDatabase()
        {
            OutputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DatabaseDirectory);
            var mdfFilename = $"{DatabaseName}.mdf";
            DatabaseMdfPath = Path.Combine(OutputFolder, mdfFilename);
            DatabaseLogPath = Path.Combine(OutputFolder, $"{DatabaseName}_log.ldf");

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            // If the database does not already exist, create it.
            var connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    DetachDatabase(true);

                    var sb = new StringBuilder(100);
                    sb.AppendLine($"EXECUTE (N'CREATE DATABASE {DatabaseName}");
                    sb.AppendLine($"ON PRIMARY (NAME = N''{DatabaseName}'', FILENAME = ''{DatabaseMdfPath}'')");
                    sb.AppendLine($"LOG ON (NAME = N''{DatabaseName}_log'',  FILENAME = ''{DatabaseLogPath}'')')");

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                    // Sql sometimes caches the tables when a db is created multiple times
                    // delete all tables to get a clean database
                    cmd.CommandText = "EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL' EXEC sp_MSForEachTable 'DELETE FROM ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'  EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'";
                    cmd.ExecuteNonQuery();


                }
            }

            // Open newly created, or old database.
            ConnectionString = $@"Data Source=(LocalDB)\v11.0;AttachDBFileName={DatabaseMdfPath};Initial Catalog={DatabaseName};Integrated Security=True;";
        }

        private void DetachDatabase(bool silent)
        {
            try
            {
                // detatch the database
                var connectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"ALTER DATABASE {DatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{DatabaseName}'";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                if (silent)
                {
                    Trace.WriteLine(e);
                }
            }
            finally
            {
                if (File.Exists(DatabaseMdfPath))
                {
                    File.Delete(DatabaseMdfPath);
                }

                if (File.Exists(DatabaseLogPath))
                {
                    File.Delete(DatabaseLogPath);
                }
            }
        }
    }
}
