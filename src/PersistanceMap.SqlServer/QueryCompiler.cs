using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.SqlServer
{
    public class QueryCompiler : PersistanceMap.QueryCompiler
    {
        protected override void CompilePart(IQueryPart part, TextWriter writer, IQueryPartDecorator parent = null)
        {
            switch (part.OperationType)
            {


                default:
                    base.CompilePart(part, writer, parent);
                    break;
            }
        }

        protected override void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write(string.Format("CREATE TABLE {0} (", part.Compile()));
        }

        protected override void CompileCreateDatabase(IQueryPart part, TextWriter writer)
        {
            var database = part.Compile();

            writer.WriteLine("DECLARE @device_directory NVARCHAR(520)");
            writer.WriteLine("SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)");
            writer.WriteLine("FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1");
            //sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''Northwind'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''Northwind_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));
            writer.WriteLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''{0}'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''{0}_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));
        }
    }
}
