using System;

namespace Docller.Core.Common
{
    public interface IDocllerSession
    {
        /// <summary>
        /// Gets or sets the Value with the specified key For Request Session
        /// </summary>
        object this[string key] { get; set; }

        void Remove(string key);
        void End();
    }
}