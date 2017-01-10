using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security;
using Docller.Core.Common;
using Docller.Core.Images;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Storage;
using Microsoft.DeepZoomTools;
using StructureMap;
using Image = System.Drawing.Image;

namespace Docller.Core.Services
{
    public class SubscriptionService : ServiceBase<ISubscriptionRepository>, ISubscriptionService
    {
        public SubscriptionService(ISubscriptionRepository repository) : base(repository)
        {
        }

        public SubscriptionServiceStatus Subscribe(Customer customer)
        {
            
            Security.PopulatePassword(customer.AdminUser);

            CustomerSubInfo results = this.Repository.Subscribe(customer);
             
            //Add the User and Company in UserCache on Customer Federation
            if(results.ReturnVal == 0 && customer.CustomerId > 0)
            {
                //Insert user and company details into the customer Federation
                ICustomerSubscriptionService cust = ServiceFactory.GetCustomerSubscriptionService(customer.CustomerId);
                customer.AdminUser.CustomerPermissions = PermissionFlag.Admin;
                cust.UpdateSubscription(customer);
            }

            if (results.IsExistingUser && results.ReturnVal == 0)
            {
                return SubscriptionServiceStatus.ExistingUserNewCustomer;
            }
            else
            {
                return (SubscriptionServiceStatus)results.ReturnVal;
            }
            
        }

        public SubscriptionServiceStatus Subscribe(IEnumerable<User> users)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether [is domain URL exists] [the specified domain URL].
        /// </summary>
        /// <param name="domainUrl">The domain URL.</param>
        /// <returns>
        ///   <c>true</c> if [is domain URL exists] [the specified domain URL]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDomainUrlExists(string domainUrl)
        {
            return this.Repository.IsDomainUrlInUse(domainUrl);
        }


        
        public Customer GetCustomer(string domainUrl)
        {
            Customer customer = this.Repository.GetCustomer(domainUrl);
            if (customer == null)
                throw new SecurityException();
            return customer;
        }

        public Customer GetCustomer(long customerId)
        {
            Customer customer = this.Repository.GetCustomer(customerId);
            if (customer == null)
                throw new SecurityException();
            return customer;
        }

        public void SaveLogo(long customerId, Stream logoStream, string fileName)
        {
            ILocalStorage localStorage = Factory.GetLocalStorageProvider();
            string fullFolderPath = localStorage.EnsureCacheFolder(Constants.CustomerContainer);
            string custlogo = string.Format("Customer_{0}_Logo.png", customerId);
            string logoPath = string.Format("{0}\\{1}", fullFolderPath, custlogo);
            using (Image photo = new Bitmap(logoStream))
            {
                ImageResizer.ResizeImage(photo, logoPath, 100, 100);
            }
            IBlobStorageProvider storageProvider = Factory.GetInstance<IBlobStorageProvider>();
            using (FileStream stream = new FileStream(logoPath,FileMode.Open,FileAccess.Read))
            {
                storageProvider.UploadFile(Constants.CustomerContainer, Constants.CustomerLogoFolder, custlogo,
                    stream,
                    MIMETypes.Current[".png"]);
            }

            this.Repository.UpdateCustomer(new Customer() {CustomerId = customerId, ImageUrl = custlogo});
        }

        public string LogoFile(long customerId)
        {
            ILocalStorage localStorage = Factory.GetLocalStorageProvider();
            string fullFolderPath = localStorage.EnsureCacheFolder(Constants.CustomerContainer);
            string targetFile = string.Format("{0}\\Customer_{1}_Logo.png", fullFolderPath, customerId);
            if (!System.IO.File.Exists(targetFile))
            {
                IBlobStorageProvider storageProvider = Factory.GetInstance<IBlobStorageProvider>();
                string storagePath = string.Format("{0}{1}Customer_{2}_Logo.png", Constants.CustomerLogoFolder, storageProvider.GetPathSeparator(),customerId);
                using (
                    FileStream target = new FileStream(targetFile,
                        FileMode.Create, FileAccess.Write))
                {
                    storageProvider.DownloadToStream(Constants.CustomerContainer, storagePath, target);
                }
            }
            return targetFile;
        }
    }
}
