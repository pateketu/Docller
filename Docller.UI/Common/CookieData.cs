using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Docller.Core.Common;

namespace Docller.Common
{
    public class CookieData
    {
        public CookieData()
        {

        }

        public CookieData(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!value.StartsWith("?"))
                {
                    value = value.Insert(0, "?");
                }
                NameValueCollection propCollection = HttpUtility.ParseQueryString(value);
                if (propCollection.Count > 0)
                {
                    this.Deserialize(propCollection);
                }
            }
        }

        [Key("FU")]
        public bool IsForceAccountUpdate { get; set; }

        [Key("CID")]
        public long CustomerId { get; set; }

        [Key("DN")]
        public string DisplayName { get; set; }

        [Key("CN")]
        public string CompanyName { get; set; }
        
        public override string ToString()
        {
            StringBuilder valueBuilder = new StringBuilder();
            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string key = GetKey(propertyInfo);

                if (string.IsNullOrEmpty(key))
                    continue;

                object value = propertyInfo.GetValue(this, null);
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    valueBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", key,
                                              propertyInfo.GetValue(this, null));
                }
            }
            //Remove the First & (Ampersand)
            return valueBuilder.Remove(0, 1).ToString();
        }

        private void Deserialize(NameValueCollection propCollection)
        {
            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                string key = GetKey(propertyInfo);
                if (!string.IsNullOrEmpty(key))
                {
                    string value = propCollection[key];
                    SetPropertyValue(propertyInfo, value);
                }
            }

        }

        private void SetPropertyValue(PropertyInfo propertyInfo, string value)
        {
            object propValue = ValueSerializer.Deserialize(propertyInfo.PropertyType, value);
            if (propValue != null)
            {
                propertyInfo.SetValue(this, Convert.ChangeType(propValue, propertyInfo.PropertyType), null);
            }
        }

        private static string GetKey(PropertyInfo propertyInfo)
        {
            object[] keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false);
            if (keyAttributes.Length == 1)
            {
                return ((KeyAttribute)keyAttributes[0]).Key;
            }
            return null;
        }




    }
}