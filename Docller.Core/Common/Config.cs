using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Docller.Core.Common
{
    public static class Config
    {
        public static T GetValue<T>(string key)
        {
            string value = GetValue(key);
            
            if (!string.IsNullOrEmpty(value))
            {
                return (T)ValueSerializer.Deserialize(typeof(T), value);
            }
            return default(T);
        }

        public static string GetConnectionString()
        {
            return GetConnectionString(ConfigKeys.ConnectionStringName);
        }

        public static string GetConnectionString(string name)
        {
           return DocllerEnvironment.IsCloudDeployment
                ? GetValue(name)
                : ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }


        private static string GetValue(string key)
        {
            return CloudConfigurationManager.GetSetting(key);
        }

        
        
    }
}
