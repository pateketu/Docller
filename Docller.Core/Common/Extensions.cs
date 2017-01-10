using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Common
{
    public static class Extensions
    {
        public static bool TryGetItemAt<T>(this List<T> list, int index, out T t)
        {
            t = default(T);
            if (list.Count > 0 && (list.Count - 1) >= index)
            {
                t = list[index];
                return true;
            }
            return false;
        }

        public static DateTime TrimMilliseconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
        }

       public static bool HasPermissions(this PermissionFlag permissionFlag, PermissionFlag checkPermission)
        {
            return (permissionFlag & checkPermission) == checkPermission;
        }
      
    }
}
