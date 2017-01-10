using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Repository.Mappers
{
    public interface IReturnParameterMapper
    {
        /// <summary>
        /// Gets the return value.
        /// </summary>
        int? ReturnValue { get; }
    }
}
