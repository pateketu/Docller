using Docller.Core.Common;
using Docller.Core.Images;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using SnowMaker;
using StructureMap;
using Docller.Core.DB;
using StructureMap.Pipeline;

namespace Docller.Common
{
    public class Registry
    {
        public static void RegisterMappings()
        {
            FederationType rooFederationType = DocllerEnvironment.IsFederationEnabled
                                            ? FederationType.Root
                                            : FederationType.None;
            FederationType memberFederationType = DocllerEnvironment.IsFederationEnabled 
                                            ? FederationType.Member 
                                            : FederationType.None;
            FederationType allFederationType = DocllerEnvironment.IsFederationEnabled
                                                   ? FederationType.All
                                                   : FederationType.None;

            //ObjectFactory.With("connectionString").EqualTo(someValueAtRunTime).GetInstance<IProductProvider>();
            ObjectFactory.Initialize(
                x =>
                    {

                      x.For<Database>().Use<SqlAzureDatabase>()
                            .Ctor<string>("connectionString").Is(Config.GetConnectionString())
                            .Ctor<FederationType>().Is(FederationType.None)
                            .Ctor<string>("federationName").Is(Config.GetValue<string>(ConfigKeys.FederatioName))
                            .Ctor<string>("distributionName").Is(Config.GetValue<string>(ConfigKeys.DistributionName))
                            .Ctor<object>("federationKey").Is((object) null);

                       x.For<RetryPolicy>().Use(RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy());

                     
                        x.For<ISubscriptionRepository>()
                            .Use<SubscriptionRepository>().Ctor<FederationType>().Is(rooFederationType)
                            .Ctor<long>().Is(0);
                        x.For<ISubscriptionService>().Use<SubscriptionService>();

                        x.For<IUserRepository>().Use<UserRepository>().Ctor<FederationType>().Is(rooFederationType).Ctor
                            <long>().Is(0);
                        x.For<IUserService>().Use<UserService>();

                        x.For<IProjectRepository>().Use<ProjectRepository>().Ctor<FederationType>().Is(
                           memberFederationType).Ctor<long>().Is(0);
                        x.For<IProjectService>().Use<ProjectService>();

                        x.For<IBlobStorageProvider>().Use<AzureBlobStorageProvider>();

                        x.For<ISecurityRepository>().Use<SecurityRepository>().Ctor<FederationType>().Is(
                           memberFederationType).Ctor<long>().Is(0);
                        x.For<ISecurityService>().Use<SecurityService>();

                        x.For<ICustomerSubscriptionRepository>().Use<CustomerSubscriptionRepository>().Ctor<FederationType>().Is(
                           memberFederationType).Ctor<long>().Is(0);
                        x.For<ICustomerSubscriptionService>().Use<CustomerSubscriptionService>();

                        x.For<IStorageRepository>().Use<StorageRepository>().Ctor<FederationType>().Is(
                           memberFederationType).Ctor<long>().Is(0);
                        x.For<IStorageService>().Use<StorageService>();
                        
                        x.For<IUserSubscriptionRepository>().Use<UserSubscriptionRepository>().Ctor<FederationType>().Is(
                           allFederationType).Ctor<long>().Is(0);
                        x.For<IUserSubscriptionService>().Use<UserSubscriptionService>();
                        
                        
                        x.For<IUniqueIdGenerator>().Singleton().Use<DocllerUniqueIdGenerator>();

                        x.For<ITransmittalRepository>().Use<TransmittalRepository>().Ctor<FederationType>().Is(
                          memberFederationType).Ctor<long>().Is(0);
                        x.For<ITransmittalService>().Use<TransmittalService>();
                        x.For<IFolderContext>().HttpContextScoped().Use(
                            () => new FolderContext());
                        x.For<IDocllerContext>().HttpContextScoped().Use(
                            () => new DocllerWebContext());

                        x.For<ILocalStorage>()
                         .Singleton()
                         .Use(() => DocllerEnvironment.UseEmulatedStorage
                                        ? new LocalStorage()
                                        : new AzureLocalStorage());

                        x.For<IFileProcessor>()
                         .Use(
                             () => new LocalFileProcessor());

                        x.For<IIssueSheetProvider>().Use<IssueSheetProvider>();
                        x.For<ITransmittalNotification>().Use<TransmittalNotification>();
                        x.For<IPathMapper>().Use<ServerPathMapper>();
                        
                        x.For<IZoomableImageProvider>().Use<DeepZoomImageProvider>();
                        x.For<IDirectDownloadProvider>().Use<DirectDownloadProvider>();
                        x.For<IPreviewImageProvider>().Use<PreviewImageProvider>();
                        x.For<IGhostscriptLibraryLoader>().Singleton().Use<GhostscriptLibraryLoader>();
                        x.For<ISecurityContext>()
                         .LifecycleIs(new HttpSessionLifecycle())
                         .Use<DocllerSecurityContext>();
                    }
                );
        }
    }
}
