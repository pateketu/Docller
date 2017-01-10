using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository
{
    public static class RepositoryExtensions
    {

        /// <summary>
        /// Sets the nullable long.
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public static void SetNullableLong(this SqlDataRecord dataRecord, int ordinal, long value)
        {
            if (value > 0)
            {
                dataRecord.SetInt64(ordinal, value);
            }
            else
            {
                dataRecord.SetDBNull(ordinal);
            }
        }

        /// <summary>
        /// Sets the nullable int32.
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public static void SetNullableInt32(this SqlDataRecord dataRecord, int ordinal, int value)
        {
            if (value > 0)
            {
                dataRecord.SetInt32(ordinal, value);
            }
            else
            {
                dataRecord.SetDBNull(ordinal);
            }
        }

        /// <summary>
        /// Sets the nullable string.
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public static void SetNullableString(this SqlDataRecord dataRecord, int ordinal, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dataRecord.SetString(ordinal, value);
            }
            else
            {
                dataRecord.SetDBNull(ordinal);
            }
        }


        /// <summary>
        /// Sets the nullable GUID.
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public static void SetNullableGuid(this SqlDataRecord dataRecord, int ordinal, Guid value)
        {
            if(!value.Equals(Guid.Empty))
            {
                dataRecord.SetGuid(ordinal, value);
            }
            else
            {
                dataRecord.SetDBNull(ordinal);
            }
        }

        /// <summary>
        /// Sets the nullable decimal.
        /// </summary>
        /// <param name="dataRecord">The data record.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        public static void SetNullableDecimal(this SqlDataRecord dataRecord, int ordinal, decimal value)
        {
            if (value > 0)
            {
                dataRecord.SetDecimal(ordinal, value);
            }
            else
            {
                dataRecord.SetDBNull(ordinal);
            }
            
        }
    }
}
