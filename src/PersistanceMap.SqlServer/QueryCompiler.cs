using PersistanceMap.QueryParts;
using System.IO;
using System.Linq;

namespace PersistanceMap.SqlServer
{
    public class QueryCompiler : PersistanceMap.QueryCompiler
    {
        protected override void InitializeCompilers()
        {
            // Database
            AddCompiler(OperationType.CreateDatabase, (part, writer, container, parent) => CompileCreateDatabase(part, writer));
            AddCompiler(OperationType.CreateTable, (part, writer, container, parent) =>
            {
                CreateTable(part, writer);
                CompileChildParts(part, writer, container);
                CompileString(")", writer);
            });
            AddCompiler(OperationType.Column, (part, writer, container, parent) =>
            {
                CompileColumn(part, writer);
                AppendComma(part, writer, parent);
            });
            AddCompiler(OperationType.PrimaryColumn, (part, writer, container, parent) =>
            {
                CompilePrimaryColumn(part, writer);
                AppendComma(part, writer, parent);
            });
            AddCompiler(OperationType.PrimaryKey, (part, writer, container, parent) =>
            {
                CompileString("PRIMARY KEY (", writer);
                CompileChildParts(part, writer, container);
                CompileString(")", writer);
            });
            AddCompiler(OperationType.ForeignKey, (part, writer, container, parent) =>
            {
                CompileForeignKey(part, writer);
                AppendComma(part, writer, parent);
            });
            AddCompiler(OperationType.AlterTable, (part, writer, container, parent) => CompileFormat("ALTER TABLE {0} ", part, writer));
            AddCompiler(OperationType.DropTable, (part, writer, container, parent) => CompileFormat("DROP TABLE {0}", part, writer));
            AddCompiler(OperationType.DropColumn, (part, writer, container, parent) => CompileFormat("DROP COLUMN {0}", part, writer));
            AddCompiler(OperationType.AddColumn, (part, writer, container, parent) => CompileAddFieldPart(part, writer));

            // StoredProcedure
            AddCompiler(OperationType.Procedure, (part, writer, container, parent) =>
            {
                CompileFormat("EXEC {0} ", part, writer);
                CompileChildParts(part, writer, container);

                if (container == null || container.Parts.Last() != part)
                {
                    WriteLine(writer);
                }

            });
            AddCompiler(OperationType.Parameter, (part, writer, container, parent) => CompileParameter(part, writer, parent));
            AddCompiler(OperationType.OutputParameter, (part, writer, container, parent) => CompileOutputParameter(part, writer, parent));
            AddCompiler(OperationType.OutParameterDeclare, (part, writer, container, parent) =>
            {
                CompileFormat("DECLARE @{0}", part, writer);
                WriteLine(writer);
            });
            AddCompiler(OperationType.OutParameterSet, (part, writer, container, parent) =>
            {
                CompileFormat("SET @{0}", part, writer);
                WriteLine(writer);
            });
            AddCompiler(OperationType.OutParameterSelect, (part, writer, container, parent) => CompileOutputParameterSelect(part, writer, parent));
            AddCompiler(OperationType.OutParameterDefinition, (part, writer, container, parent) => CompileChildParts(part, writer, container));
            AddCompiler(OperationType.IncludeMember, (part, writer, container, parent) => { /* do nothing */});
        }

        private void CompileCreateDatabase(IQueryPart part, TextWriter writer)
        {
            var database = part.Compile();

            writer.WriteLine("DECLARE @device_directory NVARCHAR(520)");
            writer.WriteLine("SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)");
            writer.WriteLine("FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1");
            //sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''Northwind'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''Northwind_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));

            writer.WriteLine("EXECUTE (N'CREATE DATABASE {0}", database);
            writer.WriteLine("ON PRIMARY (NAME = N''{0}'', FILENAME = N''' + @device_directory + N'{0}.mdf'')", database);
            writer.WriteLine("LOG ON (NAME = N''{0}_log'',  FILENAME = N''' + @device_directory + N'{0}_log.ldf'')')", database);
            //writer.WriteLine("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''{0}'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''{0}_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database);



            //var sb = new StringBuilder(100);
            //sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0}", DatabaseName));
            //sb.AppendLine(string.Format("ON PRIMARY (NAME = N''{0}'', FILENAME = ''{1}'')", DatabaseName, DatabaseMdfPath));
            //sb.AppendLine(string.Format("LOG ON (NAME = N''{0}_log'',  FILENAME = ''{1}'')')", DatabaseName, DatabaseLogPath));
        }

        private void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write("CREATE TABLE {0} (", part.Compile());
        }

        private void CompileColumn(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var column = collection.GetValue(KeyValuePart.MemberName);
            var type = collection.GetValue(KeyValuePart.MemberType);
            var nullable = collection.GetValue(KeyValuePart.Nullable);

            writer.Write("{0} {1}{2}", column, type, string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL");
        }

        private void CompilePrimaryColumn(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var column = collection.GetValue(KeyValuePart.MemberName);
            var type = collection.GetValue(KeyValuePart.MemberType);
            var nullable = collection.GetValue(KeyValuePart.Nullable);
            var autoIncremtent = collection.GetValue(KeyValuePart.AutoIncrement);

            writer.Write("{0} {1} PRIMARY KEY{2}{3}",
                    column,
                    type,
                    string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL",
                    !string.IsNullOrEmpty(autoIncremtent) && autoIncremtent.ToLower() == "true" ? " AUTOINCREMENT" : "");
        }

        private void CompileForeignKey(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var column = collection.GetValue(KeyValuePart.MemberName);
            var reference = collection.GetValue(KeyValuePart.ReferenceTable);
            var referenceMember = collection.GetValue(KeyValuePart.ReferenceMember);
            //var nullable = collection.GetValue(KeyValuePart.Nullable);

            writer.Write("FOREIGN KEY({0}) REFERENCES {1}({2})", column, reference, referenceMember);
        }

        private void CompileAddFieldPart(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var column = collection.GetValue(KeyValuePart.MemberName);
            var type = collection.GetValue(KeyValuePart.MemberType);
            var nullable = collection.GetValue(KeyValuePart.Nullable);

            writer.Write("ADD {0} {1}{2}", column, type, string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL");
        }

        private void CompileParameter(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            writer.Write(part.Compile());

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        private void CompileOutputParameter(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            writer.Write("{0} OUTPUT", part.Compile());

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        private void CompileOutputParameterSelect(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
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
