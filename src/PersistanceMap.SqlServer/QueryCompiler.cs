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
        protected override void CompilePart(IQueryPart part, TextWriter writer, IQueryPartsContainer container, IQueryPartDecorator parent = null)
        {
            switch (part.OperationType)
            {
                // Database
                case OperationType.CreateDatabase:
                    CompileCreateDatabase(part, writer);
                    break;
                case OperationType.CreateTable:
                    CreateTable(part, writer);
                    break;

                // StoredProcedure
                case OperationType.Procedure:
                    CompileFormat("EXEC {0} ", part, writer);
                    CompileChildParts(part, writer, container);

                    if (container == null || container.Parts.Last() != part)
                    {
                        WriteLine(writer);
                    }

                    break;
                case OperationType.Parameter:
                    CompileParameter(part, writer, parent);
                    break;
                case OperationType.OutputParameter:
                    CompileOutputParameter(part, writer, parent);
                    break;

                case OperationType.OutParameterDeclare:
                    CompileFormat("DECLARE @{0}", part, writer);
                    WriteLine(writer);
                    break;
                case OperationType.OutParameterSet:
                    CompileFormat("SET @{0}", part, writer);
                    WriteLine(writer);
                    break;
                case OperationType.OutParameterSelect:
                    CompileOutputParameterSelect(part, writer, parent);
                    break;

                case OperationType.OutParameterDefinition:
                    CompileChildParts(part, writer, container);
                    break;

                case OperationType.IncludeMember:
                    // do nothing
                    break;


                default:
                    base.CompilePart(part, writer, container, parent);
                    return;
                    break;
            }

            //CompileChildParts(part, writer);
        }

        protected override void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write("CREATE TABLE {0} (", part.Compile());
        }

        protected override void CompileCreateDatabase(IQueryPart part, TextWriter writer)
        {
            var database = part.Compile();

            writer.WriteLine("DECLARE @device_directory NVARCHAR(520)");
            writer.WriteLine("SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)");
            writer.WriteLine("FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1");
            //sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''Northwind'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''Northwind_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));
            writer.WriteLine("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''{0}'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''{0}_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database);
        }

        private void CompileParameter(IQueryPart part, TextWriter writer, IQueryPartDecorator parent)
        {
            writer.Write(part.Compile());

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        private void CompileOutputParameter(IQueryPart part, TextWriter writer, IQueryPartDecorator parent)
        {
            writer.Write("{0} OUTPUT", part.Compile());

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        private void CompileOutputParameterSelect(IQueryPart part, TextWriter writer, IQueryPartDecorator parent)
        {
            if (parent != null && parent.Parts.FirstOrDefault(p => p.OperationType == OperationType.OutParameterSelect) == part)
            {
                WriteBlank(writer);
            }

            writer.Write("@{0} AS {0}", part.Compile());

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }
    }
}
