using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Docller.Core.Resources;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers
{
    public class ModelParameterMapper<T> : IParameterMapper, IReturnParameterMapper, IOutputParameterMapper, IFluentParameterMapper<T>, IFluentParameterMapperContext<T>
    {
        private readonly Database _database;
        private DbParameter _returnParameter;
        private readonly T _entity;
        private readonly Dictionary<string, object> _propertyMappings;
        private readonly Dictionary<string, object> _parameterOverrides;
        private readonly Dictionary<string, PropertyInfo> _outputParameterMappings;
        private readonly List<DbParameter> _outputParameters;
        private PropertyInfo _mappedPropertyInfo;
        private object _mappedPropertyValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelParameterMapper&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="t">The t.</param>
        public ModelParameterMapper(Database db, T t)
        {
            this._database = db;
            this._entity = t;
            this._propertyMappings = new Dictionary<string, object>();
            this._parameterOverrides = new Dictionary<string, object>();
            this._outputParameterMappings = new Dictionary<string, PropertyInfo>();
            this._outputParameters = new List<DbParameter>();
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        protected T Entity
        {
            get { return this._entity; }
        }

        /// <summary>
        /// Gets the return value.
        /// </summary>
        public int? ReturnValue
        {
            get
            {
                if (this._returnParameter != null)
                {
                    return Convert.ToInt32(this._returnParameter.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary/>
        /// <param name="command"/><param name="parameterValues"/>
        public virtual void AssignParameters(DbCommand command, object[] parameterValues)
        {
            this._database.DiscoverParameters(command);
            this.ExtractProperties();

            DbParameterCollection discoverdParams = command.Parameters;
            foreach (DbParameter discoverdParam in discoverdParams)
            {
                if (discoverdParam.Direction == ParameterDirection.ReturnValue)
                {
                    this._returnParameter = discoverdParam;
                    continue;
                }

                if(discoverdParam.Direction == ParameterDirection.Output
                    || discoverdParam.Direction == ParameterDirection.InputOutput)
                {
                    this._outputParameters.Add(discoverdParam);
                    continue;
                }

                string paramName = discoverdParam.ParameterName;
                //Check if we have an overrider 
                if (this._parameterOverrides.ContainsKey(paramName))
                {
                    discoverdParam.Value = this._parameterOverrides[paramName];
                }
                else
                {
                    //remove @
                    string propertyName = paramName.Remove(0, 1);
                    discoverdParam.Value = this._propertyMappings.ContainsKey(propertyName) ? this._propertyMappings[propertyName] : null;

                }
            }
        }

        /// <summary>
        /// Assigns the outpur parameters.
        /// </summary>
        public void AssignOutpurParameters()
        {
            foreach (DbParameter outputParameter in _outputParameters)
            {
                if (this._outputParameterMappings.ContainsKey(outputParameter.ParameterName))
                {
                    PropertyInfo info = this._outputParameterMappings[outputParameter.ParameterName];
                    info.SetValue(this.Entity, outputParameter.Value, null);
                }
            }
        }

        public object GetOutputParamValue(string paramName)
        {
            foreach (DbParameter parameter in _outputParameters)
            {
                if(parameter.ParameterName.Equals(paramName,StringComparison.CurrentCultureIgnoreCase))
                {
                    return parameter.Value;
                }
            }
            return null;
        }

        #region Fluent Mapper
        /// <summary>
        /// Maps the specified property selector.
        /// </summary>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns></returns>
        public IFluentParameterMapperContext<T> Map<TMember>(Expression<Func<T, TMember>> propertySelector)
        {
            MemberExpression memberExpression = propertySelector.Body as MemberExpression;
            this._mappedPropertyInfo  = memberExpression.Member as PropertyInfo;
            return this;
        }

      

        ///// <summary>
        ///// Maps a paramter by name to a property of a PROPERTY
        ///// </summary>
        ///// <typeparam name="TMember">The type of the member.</typeparam>
        ///// <typeparam name="TCMember">The type of the member.</typeparam>
        ///// <param name="propertySelector">The property selector.</param>
        ///// <param name="childProperty">The child property.</param>
        ///// <returns></returns>
        public IFluentParameterMapper<T> MapByName<TMember, TCMember>(Expression<Func<T, TMember>> mapFromProperty, Expression<Func<TMember, TCMember>> propertySelector)
        {
            MemberExpression memberExpression = mapFromProperty.Body as MemberExpression;
            object value = GetMemberValue(memberExpression, this.Entity);

            memberExpression = propertySelector.Body as MemberExpression;
            string propName;
            object propValue = GetMemberValue(memberExpression, value, out propName);
            this._parameterOverrides.Add(string.Format(CultureInfo.InvariantCulture, "@{0}", propName), propValue);
            return this;
        }

        public IFluentParameterMapperContext<T> Map<TMember, TCMember>(Expression<Func<T, TMember>> mapFromProperty, Expression<Func<TMember, TCMember>> propertySelector)
        {
            MemberExpression memberExpression = mapFromProperty.Body as MemberExpression;
            object value = GetMemberValue(memberExpression, this.Entity);

            memberExpression = propertySelector.Body as MemberExpression;
            string propName;
            object propValue = GetMemberValue(memberExpression, value, out propName);
            this._mappedPropertyValue = propValue;
            return this;
        }

        public IFluentParameterMapperContext<T> Map(object parameterValue)
        {
            this._mappedPropertyValue = parameterValue;
            return this;
        }


        /// <summary>
        /// Parameter name to map to
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <returns></returns>
        public IFluentParameterMapper<T> ToParameter(string paramName)
        {
            this._parameterOverrides.Add(paramName, this._mappedPropertyValue);
            return this;
        }

        /// <summary>
        /// Toes the output parameter.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <returns></returns>
        public IFluentParameterMapper<T> ToOutputParameter(string paramName)
        {
            this._outputParameterMappings.Add(paramName,this._mappedPropertyInfo);
            return this;
        }
        #endregion Fluent Mapper

        
        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="objectValue">The object value.</param>
        /// <returns></returns>
        private static object GetMemberValue(MemberExpression memberExpression, object objectValue)
        {
            string propName;
            return GetMemberValue(memberExpression, objectValue, out propName);
        }
        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="objectValue">The object value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static object GetMemberValue(MemberExpression memberExpression,  object objectValue, out string propertyName)
        {
            if (memberExpression == null) throw new ArgumentException(StringResources.ExceptionArgumentMustBePropertyExpression, "memberExpression");
            PropertyInfo property = memberExpression.Member as PropertyInfo;
            if (property == null) throw new ArgumentException(StringResources.ExceptionArgumentMustBePropertyExpression, "memberExpression");
            propertyName = property.Name;
            return property.GetValue(objectValue, null);
        }

        /// <summary>
        /// Extracts the properties.
        /// </summary>
        private void ExtractProperties()
        {
            var properties =
              from property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
              where IsAutoMappableProperty(property)
              select property;

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    this._propertyMappings.Add(propertyInfo.Name,
                                              (int)propertyInfo.GetValue(this._entity, null));
                }
                else
                {
                    this._propertyMappings.Add(propertyInfo.Name, propertyInfo.GetValue(this._entity, null));
                }

            }
        }

        /// <summary>
        /// Determines whether [is auto mappable property] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if [is auto mappable property] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAutoMappableProperty(PropertyInfo property)
        {
            return property.CanRead
              && property.GetIndexParameters().Length == 0
              && !IsCollectionType(property.PropertyType);
        }

        /// <summary>
        /// Determines whether [is collection type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is collection type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCollectionType(Type type)
        {
            // string implements IEnumerable, but for our purposes we don't consider it a collection.
            if (type == typeof(string)) return false;

            var interfaces = from inf in type.GetInterfaces()
                             where inf == typeof(IEnumerable) ||
                                 (inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                             select inf;
            return interfaces.Count() != 0;
        }


        
    }
}
