using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Docller.Core.Repository.Mappers
{
    public interface IFluentParameterMapper<TResult>
    {

        /// <summary>
        /// Maps the output parameter.
        /// </summary>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns></returns>
        IFluentParameterMapperContext<TResult> Map<TMember>(Expression<Func<TResult, TMember>> propertySelector);

        /// <summary>
        /// Maps parameter by name
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <typeparam name="TCMember">The type of the C member.</typeparam>
        /// <param name="mapFromProperty">The map from property.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns></returns>
        IFluentParameterMapper<TResult> MapByName<TMember, TCMember>(
            Expression<Func<TResult, TMember>> mapFromProperty, Expression<Func<TMember, TCMember>> propertySelector);


        /// <summary>
        /// Maps the specified map from property.
        /// </summary>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <typeparam name="TCMember">The type of the C member.</typeparam>
        /// <param name="mapFromProperty">The map from property.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns></returns>
        IFluentParameterMapperContext<TResult> Map<TMember, TCMember>(
            Expression<Func<TResult, TMember>> mapFromProperty, Expression<Func<TMember, TCMember>> propertySelector);

        /// <summary>
        /// Maps the specified parameter value.
        /// </summary>
        /// <param name="parameterValue">The parameter value.</param>
        /// <returns></returns>
        IFluentParameterMapperContext<TResult> Map(object parameterValue);
    }

    public interface IFluentParameterMapperContext<TResult>
    {
        /// <summary>
        /// Name of the Parameter
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <returns></returns>
        IFluentParameterMapper<TResult> ToParameter(string paramName);

        /// <summary>
        /// Toes the output parameter.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <returns></returns>
        IFluentParameterMapper<TResult> ToOutputParameter(string paramName);

    }
}
