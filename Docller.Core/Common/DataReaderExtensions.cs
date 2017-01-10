using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public static class DataReaderExtensions
    {
        public static string GetNullableString(this IDataRecord reader, int i)
        {
            return reader.IsDBNull(i) ? null : reader.GetString(i);
        }

        public static long GetNullableLong(this IDataRecord reader, int i)
        {
            return reader.IsDBNull(i) ? 0 : reader.GetInt64(i);
        }

        public static long GetNullableInt(this IDataRecord reader, int i)
        {
            return reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
        }

        public static DateTime GetNullableDateTime(this IDataRecord reader, int i)
        {
            return reader.IsDBNull(i) ? DateTime.MinValue : reader.GetDateTime(i);
        }
    }
}
