using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryParts;
using PersistenceMap.Interception;

namespace PersistenceMap
{
    /// <summary>
    /// Compiels the QueryParts to a sql string
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {
        readonly Dictionary<OperationType, Action<IQueryPart, TextWriter, IQueryPartsContainer, IQueryPart>> _compilers = new Dictionary<OperationType, Action<IQueryPart, TextWriter, IQueryPartsContainer, IQueryPart>>();
        private HashSet<IQueryPart> _compiledParts;

        public QueryCompiler()
        {
            Initialize();
        }

        private void Initialize()
        {
            AddCompiler(OperationType.None, (part, writer, container, parent) => writer.Write(part.Compile()));
            AddCompiler(OperationType.IgnoreColumn, (part, writer, container, parent) => {/* do nothing */});
            AddCompiler(OperationType.IncludeMember, (part, writer, container, parent) => {/* do nothing */});
            AddCompiler(OperationType.Select, (part, writer, container, parent) => CompileString("SELECT", writer));
            AddCompiler(OperationType.Include, (part, writer, container, parent) => CompileField(part, writer, parent));
            AddCompiler(OperationType.Field, (part, writer, container, parent) => CompileField(part, writer, parent));
            AddCompiler(OperationType.Count, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                CompileFieldFunction("COUNT", part, writer, parent);
            });
            AddCompiler(OperationType.Max, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                CompileFieldFunction("MAX", part, writer, parent);
            });
            AddCompiler(OperationType.Min, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                CompileFieldFunction("MIN", part, writer, parent);
            });
            AddCompiler(OperationType.From, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                WriteLine(writer);
                CompileFrom(part, writer);
            });
            AddCompiler(OperationType.Join, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                WriteLine(writer);
                CompileJoin(part, writer);
            });
            AddCompiler(OperationType.On, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                CompileCommand("ON", part, writer);
            });
            AddCompiler(OperationType.And, (part, writer, container, parent) =>
            {
                writer.WriteLine();
                WriteBlank(writer);
                CompileCommand("AND", part, writer);
            });
            AddCompiler(OperationType.Or, (part, writer, container, parent) =>
            {
                writer.WriteLine();
                WriteBlank(writer);
                CompileCommand("OR", part, writer);
            });
            AddCompiler(OperationType.Where, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                writer.WriteLine();
                CompileCommand("WHERE", part, writer);
            });
            AddCompiler(OperationType.GroupBy, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                WriteLine(writer);
                //TODO: add table name?
                CompileCommand("GROUP BY", part, writer);
            });
            AddCompiler(OperationType.ThenBy, (part, writer, container, parent) =>
            {
                //TODO: add table name?
                CompileFormat(", {0}", part, writer);
            });
            AddCompiler(OperationType.OrderBy, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                WriteLine(writer);
                //TODO: add table name?
                CompileFormat("ORDER BY {0} ASC", part, writer);
            });
            AddCompiler(OperationType.OrderByDesc, (part, writer, container, parent) =>
            {
                WriteBlank(writer);
                WriteLine(writer);
                //TODO: add table name?
                CompileFormat("ORDER BY {0} DESC", part, writer);
            });
            AddCompiler(OperationType.ThenByAsc, (part, writer, container, parent) =>
            {
                //TODO: add table name?
                CompileFormat(", {0} ASC", part, writer);
            });
            AddCompiler(OperationType.ThenByDesc, (part, writer, container, parent) =>
            {
                //TODO: add table name?
                CompileFormat(", {0} DESC", part, writer);
            });

            AddCompiler(OperationType.Insert, (part, writer, container, parent) =>
            {
                CompileFormat("INSERT INTO {0} (", part, writer);
                CompileChildParts(part, writer, container);
                CompileString(")", writer);
            });
            AddCompiler(OperationType.Values, (part, writer, container, parent) =>
            {
                CompileString(" VALUES (", writer);
                CompileChildParts(part, writer, container);
                CompileString(")", writer);
            });
            AddCompiler(OperationType.InsertMember, (part, writer, container, parent) =>
            {
                CompileValue(part, writer);
                AppendComma(part, writer, parent);
            });
            AddCompiler(OperationType.InsertValue, (part, writer, container, parent) =>
            {
                CompileValue(part, writer);
                AppendComma(part, writer, parent);
            });
            AddCompiler(OperationType.Update, (part, writer, container, parent) => CompileFormat("UPDATE {0} SET ", part, writer));
            AddCompiler(OperationType.UpdateValue, (part, writer, container, parent) => CompileMemberEqualsValuePart(part, writer, parent));
            AddCompiler(OperationType.Delete, (part, writer, container, parent) => CompileFormat("DELETE FROM {0}", part, writer));

            InitializeCompilers();
        }

        /// <summary>
        /// Initialize the Compilers needed to compile the operation
        /// </summary>
        protected virtual void InitializeCompilers()
        {
        }

        /// <summary>
        /// Compile IQueryPartsContainer to a QueryString
        /// </summary>
        /// <param name="container">The container containing all queryparts to compile to sql</param>
        /// <param name="interceptors">The collection of interceptors</param>
        /// <returns>The Compiled query</returns>
        public virtual CompiledQuery Compile(IQueryPartsContainer container, InterceptorCollection interceptors)
        {
            _compiledParts = new HashSet<IQueryPart>();

            if (container.AggregatePart != null)
            {
                var interception = new InterceptionHandler(interceptors, container.AggregatePart.EntityType);
                interception.ExecuteBeforeCompile(container);
            }

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

        protected virtual void CompilePart(IQueryPart part, TextWriter writer, IQueryPartsContainer container, IQueryPart parent = null)
        {
            var compiler = _compilers[part.OperationType];
            compiler(part, writer, container, parent);

            CompileChildParts(part, writer, container);
        }

        protected void AddCompiler(OperationType operation, Action<IQueryPart, TextWriter, IQueryPartsContainer, IQueryPart> compiler)
        {
            _compilers[operation] = compiler;
        }

        private void CompileMemberEqualsValuePart(IQueryPart part, TextWriter writer, IQueryPart parent)
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

        #region Query

        protected void WriteBlank(TextWriter writer)
        {
            writer.Write(" ");
        }

        protected void WriteLine(TextWriter writer)
        {
            writer.WriteLine();
        }

        protected void CompileValue(IQueryPart part, TextWriter writer)
        {
            writer.Write(part.Compile());
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

        private void CompileField(IQueryPart part, TextWriter writer, IQueryPart parent)
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

        private void CompileFieldFunction(string function, IQueryPart part, TextWriter writer, IQueryPart parent)
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
            {
                writer.Write(",");
            }
        }

        private void CompileCommand(string command, IQueryPart part, TextWriter writer)
        {
            writer.Write("{0} {1}", command, part.Compile());
        }

        protected void CompileFormat(string format, IQueryPart part, TextWriter writer)
        {
            writer.Write(format, part.Compile());
        }

        protected void CompileString(string command, TextWriter writer)
        {
            writer.Write(command);
        }

        protected void CompileChildParts(IQueryPart part, TextWriter writer, IQueryPartsContainer container)
        {
            if (_compiledParts.AsParallel().Contains(part))
            {
                return;
            }

            _compiledParts.Add(part);

            foreach (var p in part.Parts)
            {
                CompilePart(p, writer, container, part);
            }
        }

        protected void AppendComma(IQueryPart part, TextWriter writer, IQueryPart parent)
        {
            if (parent != null && parent.Parts.Last() != part)
            {
                writer.Write(", ");
            }
        }

        #endregion
    }
}
