using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Docller.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(string key)
        {
            this.Key = key;
        }

        public string Key { get; set; }
    }
}