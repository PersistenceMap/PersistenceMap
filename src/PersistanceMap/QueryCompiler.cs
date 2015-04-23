using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Compiels the QueryParts to a sql string
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        private HashSet<IQueryPart> _compiledParts;

        /// <summary>
        /// Compile IQueryPartsContainer to a QueryString
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual CompiledQuery Compile(IQueryPartsContainer container)
        {
            _compiledParts = new HashSet<IQueryPart>();

            using (var writer = new StringWriter())
            {
                foreach (var part in container.Parts)
                {
                    CompilePart(part, writer, container);
                }

                var query = writer.ToString();

                return new CompiledQuery
                {
                    QueryString = query,
                    QueryParts = container
                };
            }
        }

        protected virtual void CompilePart(IQueryPart part, TextWriter writer, IQueryPartsContainer container, IItemsQueryPart parent = null)
        {
            switch (part.OperationType)
            {
                case OperationType.None:
                    writer.Write(part.Compile());
                    break;

                case OperationType.IgnoreColumn:
                case OperationType.IncludeMember:
                    // do nothing
                    break;

                case OperationType.Select:
                    CompileString("SELECT", writer);
                    break;

                case OperationType.Include:
                case OperationType.Field:
                    CompileField(part, writer, parent);
                    break;
                case OperationType.Count:
                    WriteBlank(writer);
                    CompileFieldFunction("COUNT", part, writer, parent);
                    break;
                case OperationType.Max:
                    WriteBlank(writer);
                    CompileFieldFunction("MAX", part, writer, parent);
                    break;
                case OperationType.Min:
                    WriteBlank(writer);
                    CompileFieldFunction("MIN", part, writer, parent);
                    break;

                case OperationType.From:
                    WriteBlank(writer);
                    WriteLine(writer);
                    CompileFrom(part, writer);
                    break;
                case OperationType.Join:
                    WriteBlank(writer);
                    WriteLine(writer);
                    CompileJoin(part, writer);
                    break;



                case OperationType.On:
                    WriteBlank(writer);
                    CompileCommand(part, writer);
                    break;
                case OperationType.And:
                case OperationType.Or:
                    writer.WriteLine();
                    WriteBlank(writer);
                    CompileCommand(part, writer);
                    break;
                case OperationType.Where:
                    WriteBlank(writer);
                    writer.WriteLine();
                    CompileCommand(part, writer);
                    break;

                case OperationType.GroupBy:
                    WriteBlank(writer);
                    WriteLine(writer);
                    //TODO: add table name?
                    CompileCommand("GROUP BY", part, writer);
                    break;
                case OperationType.ThenBy:
                    //TODO: add table name?
                    CompileFormat(", {0}", part, writer);
                    break;

                case OperationType.OrderBy:
                    WriteBlank(writer);
                    WriteLine(writer);
                    //TODO: add table name?
                    CompileFormat("ORDER BY {0} ASC", part, writer);
                    break;

                case OperationType.OrderByDesc:
                    WriteBlank(writer);
                    WriteLine(writer);
                    //TODO: add table name?
                    CompileFormat("ORDER BY {0} DESC", part, writer);
                    break;

                case OperationType.ThenByAsc:
                    //TODO: add table name?
                    CompileFormat(", {0} ASC", part, writer);
                    break;
                case OperationType.ThenByDesc:
                    //TODO: add table name?
                    CompileFormat(", {0} DESC", part, writer);
                    break;

                    


                case OperationType.Insert:
                    CompileFormat("INSERT INTO {0} (", part, writer);
                    CompileChildParts(part, writer, container);
                    CompileString(")", writer);
                    break;
                case OperationType.Values:
                    CompileString(" VALUES (", writer);
                    CompileChildParts(part, writer, container);
                    CompileString(")", writer);
                    break;
                case OperationType.InsertMember:
                case OperationType.InsertValue:
                    CompileCollectionValuePart(part, writer, parent);
                    break;

                case OperationType.Update:
                    CompileFormat("UPDATE {0} SET ", part, writer);
                    break;
                case OperationType.UpdateValue:
                    CompileUpdateValuePart(part, writer, parent);
                    break;

                case OperationType.Delete:
                    CompileFormat("DELETE FROM {0}", part, writer);
                    break;


                // Database                
                case OperationType.Column:
                case OperationType.TableKeys:
                    //TODO: NOT NICE!!!
                    CompilePartSimple(part, writer);
                    break;

                

                default:
                    throw new NotImplementedException();
                    break;
            }

            CompileChildParts(part, writer, container);
        }

        private void CompileUpdateValuePart(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            var collection = part as IValueCollectionQueryPart;
            if (collection == null)
            {
                writer.Write(part.Compile());
                return;
            }

            var key = collection.GetValue(KeyValuePart.MemberName);
            var value = collection.GetValue(KeyValuePart.Value);

            writer.Write("{0} = {1}", key, value);
            AppendComma(part, writer, parent);
        }

        protected void CompileChildParts(IQueryPart part, TextWriter writer, IQueryPartsContainer container)
        {
            if (_compiledParts.AsParallel().Contains(part))
            {
                return;
            }

            _compiledParts.Add(part);

            var decorator = part as IItemsQueryPart;
            if (decorator != null)
            {
                foreach (var p in decorator.Parts)
                {
                    CompilePart(p, writer, container, decorator);
                }
            }
        }

        private void CompileCollectionValuePart(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            writer.Write(part.Compile());

            AppendComma(part, writer, parent);
        }

        protected void AppendComma(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        #region Query

        protected void WriteBlank(TextWriter writer)
        {
            writer.Write(" ");
        }

        protected void WriteLine(TextWriter writer)
        {
            writer.WriteLine();
        }

        private void CompileJoin(IQueryPart part, TextWriter writer)
        {
            var entityMap = part as IEntityPart;
            if (entityMap == null)
            {
                writer.Write("JOIN {0}", part.Compile());
                return;
            }

            writer.Write("JOIN {0}{1}", entityMap.Entity, string.IsNullOrEmpty(entityMap.EntityAlias) ? string.Empty : string.Format(" {0}", entityMap.EntityAlias));
        }

        private void CompileFrom(IQueryPart part, TextWriter writer)
        {
            var entityMap = part as IEntityPart;
            if (entityMap == null)
            {
                writer.Write("FROM {0}", part.Compile());
                return;
            }

            writer.Write("FROM {0}{1}", entityMap.Entity, string.IsNullOrEmpty(entityMap.EntityAlias) ? string.Empty : string.Format(" {0}", entityMap.EntityAlias));
        }

        private void CompileField(IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            var field = part as IFieldPart;
            if (field == null)
            {
                // try to compile the expression
                writer.Write(part.Compile());
                return;
            }

            if (!string.IsNullOrEmpty(field.EntityAlias) || !string.IsNullOrEmpty(field.Entity))
            {
                writer.Write(" {0}.", field.EntityAlias ?? field.Entity);
            }
            else
            {
                writer.Write(" ");
            }

            writer.Write(field.Field);

            if (!string.IsNullOrEmpty(field.FieldAlias))
            {
                writer.Write(" AS {0}", field.FieldAlias);
            }

            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(",");
            }
        }

        private void CompileFieldFunction(string function, IQueryPart part, TextWriter writer, IItemsQueryPart parent)
        {
            var field = part as IFieldPart;
            if (field == null)
            {
                // try to compile the expression
                writer.Write(part.Compile());
                return;
            }

            //TODO: EntityAlias is allways null, It has to be able to be set when creating a Count expression
            writer.Write("{0}({1}) AS {2}", function, field.Field, field.FieldAlias ?? field.Field);

            if (parent.Parts.Last() != part)
                writer.Write(",");
        }

        private void CompileCommand(IQueryPart part, TextWriter writer)
        {
            writer.Write("{0} {1}", part.OperationType.ToString().ToUpper(), part.Compile());
        }

        private void CompileCommand(string command, IQueryPart part, TextWriter writer)
        {
            writer.Write("{0} {1}", command, part.Compile());
        }

        protected void CompileFormat(string format, IQueryPart part, TextWriter writer)
        {
            writer.Write(format, part.Compile());
        }

        private void CompileString(string command, TextWriter writer)
        {
            writer.Write(command);
        }

        protected void CompilePartSimple(IQueryPart part, TextWriter writer)
        {
            writer.Write(part.Compile());
        }

        #endregion
    }
}
