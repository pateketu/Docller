using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public abstract class BaseModel
    {
        /// <summary>
        /// Gets the created date.
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedDate { get; internal set; }

        /// <summary>
        /// Stroed Procedure to ExecuteNonQuery single entity
        /// </summary>
        internal abstract string InsertProc { get; }

        /// <summary>
        /// Stroed Procedure to Update single entity
        /// </summary>
        internal abstract string UpdateProc { get; }

        /// <summary>
        /// Stroed Procedure to delete single entity
        /// </summary>
        internal abstract string DeleteProc { get; }

        /// <summary>
        /// Stored Procedure to Populate the Single Entity
        /// </summary>
        internal abstract string GetProc { get; }
        
    }
}
