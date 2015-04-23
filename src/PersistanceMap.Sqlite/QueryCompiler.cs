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
                    CompileAddFieldPart(part, writer);
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

            writer.Write("ADD COLUMN {0} {1}{2}", column, type, string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL");
        }
    }
}
