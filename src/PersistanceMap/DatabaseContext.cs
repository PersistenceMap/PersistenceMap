﻿using PersistanceMap.Mapping;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;

namespace PersistanceMap
{
    /// <summary>
    /// Internal implementation of the Database Context
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        public DatabaseContext(IContextProvider provider)
        {
            ContextProvider = provider;
        }

        public IContextProvider ContextProvider { get; private set; }

        public IEnumerable<T> Execute<T>(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                return Map<T>(reader);
            }
        }

        public void Execute(CompiledQuery compiledQuery)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                // make sure Disposed is called on reader!
            }
        }

        public void Execute(CompiledQuery compiledQuery, params Action<IReaderContext>[] expressions)
        {
            using (var reader = ContextProvider.Execute(compiledQuery.QueryString))
            {
                foreach (var expression in expressions)
                {
                    // invoke expression with the reader
                    expression.Invoke(reader);

                    // read next resultset
                    if(reader.DataReader.IsClosed || !reader.DataReader.NextResult())
                        break;
                }
            }
        }

        /// <summary>
        /// Maps the output from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IReaderContext reader)
        {
            return Mapper.Map<T>(reader);
        }

        /// <summary>
        /// Maps the output from the reader to the provided fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public IEnumerable<T> Map<T>(IReaderContext reader, FieldDefinition[] fields)
        {
            return Mapper.Map<T>(reader, fields);
        }

        private MappingStrategy _mapper;
        /// <summary>
        /// Gets or sets a MappingStrategy
        /// </summary>
        public MappingStrategy Mapper
        {
            get
            {
                if (_mapper == null)
                    _mapper = new MappingStrategy();

                return _mapper;
            }
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~DatabaseContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
