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
                case OperationType.DropField:
                    CompileFormat("DROP COLUMN {0}", part, writer);
                    break;
                case OperationType.AddColumn:
                    CompileAddFieldPart(part, writer);
                    break;

                default:
                    base.CompilePart(part, writer, container, parent);
                    break;
            }
        }

        private void CreateTable(IQueryPart part, TextWriter writer)
        {
            writer.Write(string.Format("CREATE TABLE IF NOT EXISTS {0} (", part.Compile()));
        }

        private void CompileAddFieldPart(IQueryPart part, TextWriter writer)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var column = collection.GetValue(KeyValuePart.Member);
            var type = collection.GetValue(KeyValuePart.MemberType);
            var nullable = collection.GetValue(KeyValuePart.Nullable);

            writer.Write("ADD COLUMN {0} {1}{2}", column, type, string.IsNullOrEmpty(nullable) || nullable.ToLower() == "true" ? "" : " NOT NULL");
        }
    }
}
