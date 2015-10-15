using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Text;

namespace PersistanceMap.Test.LocalDb
{
    public class LocalDbManager : IDisposable
    {
        static LocalDbManager()
        {
            DatabaseDirectory = "Data";
        }

        public LocalDbManager(string databaseName = null)
        {
            DatabaseName = string.IsNullOrWhiteSpace(databaseName) ? Guid.NewGuid().ToString("N").ToLower() : databaseName;

            try
            {
                CreateDatabase();
                Trace.WriteLine(string.Format("DatabaseManager created the Local Database {0}", Path.Combine(DatabaseDirectory, DatabaseName)));
            }
            catch (SqlException e)
            {
                Trace.WriteLine(string.Format("DatabaseManager could not create Database {0}.\n\r{1}", DatabaseName, e.Message));
                throw e;
            }
        }

        public static string DatabaseDirectory { get; set; }

        public string ConnectionString { get; private set; }

        public string DatabaseName { get; private set; }

        public string OutputFolder { get; private set; }

        public string DatabaseMdfPath { get; private set; }

        public string DatabaseLogPath { get; private set; }

        public IDbConnection OpenConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public void Dispose()
        {
            DetachDatabase();
        }

        public void ExecuteString(string script)
        {
            var sqlConnection = (SqlConnection)OpenConnection();

            var server = new Server(new ServerConnection(sqlConnection));
            server.ConnectionContext.ExecuteNonQuery(script);
            sqlConnection.Close();
        }

        private void CreateDatabase()
        {
            OutputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DatabaseDirectory);
            var mdfFilename = string.Format("{0}.mdf", DatabaseName);
            DatabaseMdfPath = Path.Combine(OutputFolder, mdfFilename);
            DatabaseLogPath = Path.Combine(OutputFolder, string.Format("{0}_log.ldf", DatabaseName));

            // Create Data Directory If It Doesn't Already Exist.
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }

            // If the database does not already exist, create it.
            var connectionString = string.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    DetachDatabase();

                    var sb = new StringBuilder(100);
                    sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0}", DatabaseName));
                    sb.AppendLine(string.Format("ON PRIMARY (NAME = N''{0}'', FILENAME = ''{1}'')", DatabaseName, DatabaseMdfPath));
                    sb.AppendLine(string.Format("LOG ON (NAME = N''{0}_log'',  FILENAME = ''{1}'')')", DatabaseName, DatabaseLogPath));

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();

                    // Sql sometimes caches the tables when a db is created multiple times
                    // delete all tables to get a clean database
                    cmd.CommandText = "EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL' EXEC sp_MSForEachTable 'DELETE FROM ?'  EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'  EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'";
                    cmd.ExecuteNonQuery();


                }
            }

            // Open newly created, or old database.
            ConnectionString = string.Format(@"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", DatabaseName, DatabaseMdfPath);
        }

        private void DetachDatabase()
        {
            try
            {
                // detatch the database
                var connectionString = string.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'", DatabaseName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqle)
            {
                Trace.WriteLine(sqle);

                try
                {
                    // sometimes SqlServer caches the database
                    // try to delete it if it is cached
                    var connectionString = string.Format(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = string.Format("DROP DATABASE {0}; exec sp_detach_db '{0}'", DatabaseName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
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
