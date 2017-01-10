using Docller.Core.Storage;
using Docller.UI.Common;
using log4net.Appender;
using log4net.Core;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Docller.Core.Common
{
    //http://stackoverflow.com/questions/11319319/log4net-bufferingforwardingappender-performance-issue
    public class AzureTableAppender : AppenderSkeleton
    {
        private TableServiceContext _context;
        private const string TableName = "Log4Net";
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            var storageAccount = StorageHelper.DiagnosticsStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient();
            var cloudTable = tableClient.GetTableReference(TableName);
            cloudTable.CreateIfNotExists();
            _context = tableClient.GetTableServiceContext();
        }

        protected override void Append(LoggingEvent e)
        {
            _context.AddObject(TableName, new LogEventEntity
            {
                Exception = e.GetExceptionString(),
                Level = e.Level.Name,
                LoggerName = e.LoggerName,
                Message = e.RenderedMessage,
                RoleInstance = DocllerEnvironment.CurrentRoleId
            });
            _context.SaveChanges();
        }
    }
}