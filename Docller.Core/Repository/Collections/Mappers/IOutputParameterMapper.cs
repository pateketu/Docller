using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Repository.Mappers
{
    interface IOutputParameterMapper
    {
        /// <summary>
        /// Assigns the outpur parameters.
        /// </summary>
        void AssignOutpurParameters();

        object GetOutputParamValue(string paramName);
    }
}
