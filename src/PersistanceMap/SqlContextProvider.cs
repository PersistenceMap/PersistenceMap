using System.Data;
using PersistanceMap.Compiler;
using PersistanceMap.Internals;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PersistanceMap
{
    public class SqlContextProvider : IContextProvider
    {
        public SqlContextProvider(string connectionstring)
        {
            connectionstring.EnsureArgumentNotNullOrEmpty(connectionstring);

            ConnectionString = connectionstring;
        }

        public string ConnectionString { get; private set; }

        private IExpressionCompiler _expressionCompiler;

        public virtual IExpressionCompiler ExpressionCompiler
        {
            get
            {
                if (_expressionCompiler == null)
                    _expressionCompiler = new SqlExpressionCompiler();

                return _expressionCompiler;
            }
        }

        public virtual IReaderContext Execute(string query)
        {
            SqlConnection connection;
            SqlCommand command;

            //TestExecuteProc();

            connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(query, connection);
                
                return new SqlContextReader(command.ExecuteReader(), connection, command);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }


        //public void TestExecuteProc()
        //{
        //    SqlConnection connection;
        //    SqlCommand command;

        //    connection = new SqlConnection(ConnectionString);
        //    try
        //    {
        //        connection.Open();
        //        command = new SqlCommand("SalesByYear", connection);
        //        command.CommandType = CommandType.StoredProcedure;

        //        var param1 = command.Parameters.Add("Beginning_Date", SqlDbType.DateTime);
        //        param1.Value = new DateTime(1980, 1, 1);
        //        param1.Direction = ParameterDirection.Output;


        //        var param2 = command.Parameters.Add("Ending_Date", SqlDbType.DateTime);
        //        param2.Value = DateTime.Today;
        //        param2.Direction = ParameterDirection.Input;


        //        var reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            Console.WriteLine("From first SQL - " + reader.GetValue(0) + " - " + reader.GetValue(1));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(ex);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public IReaderContext ExecuteNonQuery(string query)
        {
            SqlConnection connection;
            SqlCommand command;

            connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(query, connection);
                
                command.ExecuteNonQuery();

                return new SqlContextReader(null, connection, command);

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }
    }
}
