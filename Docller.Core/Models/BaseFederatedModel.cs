using System;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public abstract class BaseFederatedModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        /// <value>
        /// The customer id.
        /// </value>
        [JsonIgnore]
        public Int64 CustomerId { get; set; }

        [JsonIgnore]
        public string CustomerName { get; set; }
    }
}