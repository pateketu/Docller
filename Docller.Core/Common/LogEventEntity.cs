using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Docller.UI.Common
{
    public class LogEventEntity : TableServiceEntity
    {
        public LogEventEntity()
        {
            var dateTime = DateTime.UtcNow;
            PartitionKey = string.Format("{0}-{1}", dateTime.Month, dateTime.Year);
            RowKey = string.Format("{0:dd HH:mm:ss.fff}-{1}", dateTime, Guid.NewGuid());
        }
        public string Exception { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string RoleInstance { get; set; }

      
    }
}