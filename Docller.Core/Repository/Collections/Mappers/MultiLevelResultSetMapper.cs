using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers
{
    public abstract class MultiLevelResultSetMapper<TResult> : IResultSetMapper<TResult>, IFluentMultiLevelResultMapper<TResult> where TResult : new()
    {
        private readonly Dictionary<int, Type> _levelTypeMappings;
        private readonly Dictionary<Type, object> _typeRowMappers;
        private int _nextKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLevelResultSetMapper&lt;T&gt;"/> class.
        /// </summary>
        protected MultiLevelResultSetMapper()
        {
            this._levelTypeMappings = new Dictionary<int, Type>();
            this._typeRowMappers = new Dictionary<Type, object>();
        }

        /// <summary>
        /// When implemented by a class, returns an enumerable of <typeparamref name="TResult"/> based on <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Data.IDataReader"/> to map.</param>
        /// <returns>
        /// The enurable of <typeparamref name="TResult"/> that is based on <paramref name="reader"/>.
        /// </returns>
        public IEnumerable<TResult> MapSet(IDataReader reader)
        {
            Dictionary<Type, IEnumerable> results  = new Dictionary<Type, IEnumerable>();
            Type type;
            using (reader)
            {
                int readerCount = 1;
                type = this._levelTypeMappings[readerCount];
                results.Add(type, ProcessReader(readerCount, reader));
                while (reader.NextResult())
                {
                    readerCount++;
                    type = this._levelTypeMappings[readerCount];
                    results.Add(type, ProcessReader(readerCount, reader));
                }
            }
            return null;
        }

        
        /// <summary>
        /// Adds the type mapping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeRowMapper">The type row mapper.</param>
        /// <remarks>
        /// This method must be called in an order in which Type should be mapped to a returend DataReader
        /// </remarks>
        public IFluentMultiLevelResultMapper<TResult> Map(IRowMapper<TResult> typeRowMapper)
        {
            _nextKey++;
            this._levelTypeMappings.Add(_nextKey, typeof(TResult));
            this._typeRowMappers.Add(typeof(TResult), typeRowMapper);
            return this;
        }

        /// <summary>
        /// Processes the reader.
        /// </summary>
        /// <param name="readerCount">The reader count.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private IEnumerable ProcessReader(int readerCount, IDataReader reader)
        {
            Type type = this._levelTypeMappings[readerCount];
            MethodInfo readMethod =
                this.GetType().GetMethod("Read", BindingFlags.Instance | BindingFlags.NonPublic).
                    MakeGenericMethod(new Type[] { type });
            IEnumerable levelResult =
                readMethod.Invoke(this, new object[] { reader }) as IEnumerable;
            return levelResult;
            //childResults.Add(type, levelResult);
        }


        /// <summary>
        /// Reads the specified level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private IEnumerable<T> Read<T>(IDataReader reader)
        {
            IRowMapper<T> rowMapper = this._typeRowMappers[typeof(T)] as IRowMapper<T>;
            List<T> list = new List<T>();
            while (reader.Read())
            {
                //Not using the yield return technique as we are closing the reader before build the objectgraph
                //see using(reader) in MapSet Method
                //yield return rowMapper.MapRow(reader);
                list.Add(rowMapper.MapRow(reader));
            }
            return list;
        }


    }

    public interface IFluentMultiLevelResultMapper<TResult> where TResult : new()
    {
        IFluentMultiLevelResultMapper<TResult> Map(IRowMapper<TResult> typeRowMapper);
    }
}
