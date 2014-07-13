using PersistanceMap.Compiler;
using PersistanceMap.Internals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        IExpressionCompiler _expressionCompiler;
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

            connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(query, connection);

                /*
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    MessageBox.Show("From first SQL - " + sqlReader.GetValue(0) + " - " + sqlReader.GetValue(1));
                }

                sqlReader.NextResult();

                while (sqlReader.Read())
                {
                    MessageBox.Show("From second SQL - " + sqlReader.GetValue(0) + " - " + sqlReader.GetValue(1));
                }

                sqlReader.NextResult();

                while (sqlReader.Read())
                {
                    MessageBox.Show("From third SQL - " + sqlReader.GetValue(0) + " - " + sqlReader.GetValue(1));
                }

                sqlReader.Close();
                sqlCmd.Dispose();
                sqlCnn.Close();
                */

                return new SqlContextReader(command.ExecuteReader(), connection, command);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }
    }
}
