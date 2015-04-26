using PersistanceMap.QueryParts;
using System;
using System.IO;

namespace PersistanceMap.Sqlite
{
    public class QueryCompiler : PersistanceMap.QueryCompiler
    {
        protected override void CompilePart(IQueryPart part, TextWriter writer, IQueryPartsContainer container, IItemsQueryPart parent = null)
        {
            switch (part.OperationType)
            {
                case OperationType.CreateDatabase:
                    throw new ArgumentException("Sqlite Database is created automaticaly");
                    break;
                case OperationType.CreateTable:
                    CreateTable(part, writer);
                    CompileChildParts(part, writer, container);
                    CompileString(")", writer);
                    break;
                case OperationType.Column:
                    CompileColumn(part, writer);
                    AppendComma(part, writer, parent);
                    break;
                case OperationType.PrimaryColumn:
                    CompilePrimaryColumn(part, writer);
                    AppendComma(part, writer, parent);
                    break;
                case OperationType.ForeignKey:
                    CompileForeignKey(part, writer);
                    AppendComma(part, writer, parent);
                    break;

                case OperationType.AlterTable:
                    CompileFormat("ALTER TABLE {0} ", part, writer);
                    break;
                case OperationType.DropTable:
                    CompileFormat("DROP TABLE {0}", part, writer);
                    break;
                case OperationType.DropColumn:
                    CompileFormat("DROP COLUMN {0}", part, writer);
                    break;
                case OperationType.AddColumn:
                    CompileAddColumnPart(part, writer);
                    break;
                case OperationType.RenameTable:
                    RenameTable(part, writer);
                    break;

                default:
                    base.CompilePart(part, writer, container, parent);
                    break;
            }
        }

        private void RenameTable(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var original = collection.GetValue(KeyValuePart.Key);
            var value = collection.GetValue(KeyValuePart.Value);

            writer.Write("ALTER TABLE {0} RENAME TO {1}", original, value);
        }

        private void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write("CREATE TABLE IF NOT EXISTS {0} (", part.Compile());
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

        private void CompileAddColumnPart(IQueryPart part, TextWriter writer)
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

            writer.Write("ADD COLUMN {0} {1}{2}", column, type, string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL");
        }
    }
}
