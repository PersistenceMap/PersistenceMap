using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    public interface IDatabaseContext : IDisposable
    {
        IContextProvider ContextProvider { get; }

        IEnumerable<T> Execute<T>(CompiledQuery compiledQuery);

        void Execute(CompiledQuery compiledQuery);

        void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions);

        /// <summary>
        /// Maps the output from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        IEnumerable<T> Map<T>(IReaderContext reader);

        /// <summary>
        /// Maps the output from the reader to the provided fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        IEnumerable<T> Map<T>(IReaderContext reader, FieldDefinition[] fields);
    }
}
