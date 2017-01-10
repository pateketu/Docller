using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Microsoft.WindowsAzure.Storage;


namespace Docller.Core.Storage
{
    public static class StorageHelper
    {
        public static DateTimeOffset SnapshotTime;
        public static string BlobContainerFormat
        {
            get { return "project-{0}-{1}-blobs"; }
        }

        public static string CadAttachmentsPath 
        {
            get { return "_cad"; }
        }

        public static string MarkUpPath
        {
            get { return "_m"; }
           
        }


        public static CloudStorageAccount StorageAccount
        {
            get
            {

                CloudStorageAccount cloudStorageAccount = DocllerEnvironment.UseEmulatedStorage
                                                          ? CloudStorageAccount.DevelopmentStorageAccount
                                                          : CloudStorageAccount.Parse(
                                                              Config.GetConnectionString(ConfigKeys.AzureStorageConnectionString));
                return cloudStorageAccount;
            }

        }

        public static CloudStorageAccount DiagnosticsStorageAccount
        {
            get
            {

                CloudStorageAccount cloudStorageAccount = DocllerEnvironment.UseEmulatedStorage
                                                          ? CloudStorageAccount.DevelopmentStorageAccount
                                                          : CloudStorageAccount.Parse(
                                                              Config.GetConnectionString(ConfigKeys.DiagnosticsConnectionString));
                return cloudStorageAccount;
            }

        }

    }
}
