using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Docller.Core.Common
{
    public static class DocllerEnvironment
    {
        private static bool? _isFederation;
        private static bool? _isLocal;
        private static bool? _isHttpCache;
        private static bool? _isAzureStorage;


        public static bool IsCloudDeployment
        {
            get { return RoleEnvironment.IsAvailable; }
        }

        public static bool UseEmulatedStorage
        {
            get
            {
                if (!IsCloudDeployment)
                    return true; //

                return RoleEnvironment.IsEmulated;

            }
        }

        public static  bool UseAzureBlobStorage
        {
            get
            {
                if (_isAzureStorage == null)
                {
                    _isAzureStorage = Config.GetValue<bool>(ConfigKeys.AzureBlobStorage);
                }
                return _isAzureStorage.Value;
            }

        }


        /// <summary>
        /// Gets a value indicating whether this instance is federation enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is federation enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsFederationEnabled
        {
            get
            {
                if (_isFederation == null)
                {
                    _isFederation = Config.GetValue<bool>(ConfigKeys.FederationEnabled);
                }
                return _isFederation.Value;
            }

        }

        public static bool UseHttpCache
        {
            get
            {

                if (_isHttpCache == null)
                {
                    _isHttpCache = Config.GetValue<bool>(ConfigKeys.UseHttpCache);
                }
                return _isHttpCache.Value;
            }
        }

        public static string CurrentRoleId
        {
            get
            {
                if (IsCloudDeployment)
                {
                    return RoleEnvironment.CurrentRoleInstance.Id;
                }
                return "No Role";
            }
        }
    }
}
