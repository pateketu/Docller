using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Docller.Common;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Models;
using Docller.Tests;
using Docller.UI.Common;
using Docller.UnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using Telerik.JustMock;
using ControllerBase = System.Web.Mvc.ControllerBase;

namespace Docller.UnitTests
{
    [TestClass]
    public class ControllerFixture:FixtureBase
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            RegisterMappings();
        }
        public static ControllerContext GetPostMockControllerContext(NameValueCollection forms = null,
                                                                     NameValueCollection querystrings = null)
        {
            return GetMockControllerContext("POST", false, null, forms, querystrings, null);
        }

        public static ControllerContext GetMockControllerContext(string httpMethod, bool isAjaxRequest, HttpRequestBase httpRequestBase, NameValueCollection form, NameValueCollection querystrings, HttpSessionStateBase httpSessionStateBase)
        {
            NameValueCollection headerCollection = new NameValueCollection();

            // Add header value for ajax requests
            if (isAjaxRequest) headerCollection.Add("X-Requested-With", "XMLHttpRequest");

            if (httpRequestBase == null)
            {
                httpRequestBase = Mock.Create<HttpRequestBase>();
                Mock.Arrange(() => httpRequestBase.HttpMethod).Returns(httpMethod);
                Mock.Arrange(() => httpRequestBase.Headers).Returns(headerCollection);
                Mock.Arrange(() => httpRequestBase.Form).Returns(form);
                Mock.Arrange(() => httpRequestBase.QueryString).Returns(querystrings);
                Mock.Arrange(() => httpRequestBase.IsAuthenticated).Returns(true);

            }

            if (httpSessionStateBase == null)
            {
                httpSessionStateBase = Mock.Create<HttpSessionStateBase>();
            }

            HttpContextBase mockHttpContext = Mock.Create<HttpContextBase>();
            Mock.Arrange(() => mockHttpContext.Request).Returns(httpRequestBase);
            Mock.Arrange(() => mockHttpContext.Session).Returns(httpSessionStateBase);

            HttpResponseBase responseBase = Mock.Create<HttpResponseBase>();
            Mock.Arrange(() => mockHttpContext.Response).Returns(responseBase);
            ControllerBase controllerBase = Mock.Create<ControllerBase>();
            return new ControllerContext(mockHttpContext, new RouteData(), controllerBase);
        }

        //[TestMethod]
        public void Verify_SaveTransmittal()
        {
            IProjectService projectService = Mock.Create<IProjectService>();
            Mock.Arrange(() => projectService.GetProjectStatuses(Arg.IsAny<long>())).Returns(new List<Status>());

            ICustomerSubscriptionRepository repository = Mock.Create<ICustomerSubscriptionRepository>();
            IProjectRepository projectRepository = Mock.Create<IProjectRepository>();
            ObjectFactory.Initialize(x =>
                {
                    x.For<ITransmittalService>().Use<MockTransmittalService>();
                    x.For<IDocllerContext>().Use(new MockContext("fubar", 1));
                    x.For<ITransmittalRepository>().Use<MockTransmittalRepository>();
                    x.For<ICustomerSubscriptionRepository>().Use(repository);
                    x.For<ICustomerSubscriptionService>().Use<MockCustomerSubscriptionService>();
                    x.For<IProjectRepository>().Use(projectRepository);
                    x.For<IProjectService>().Use(projectService);

                });

            ProjectController projectController = new ProjectController();
            ViewResult result = (ViewResult)projectController.SaveTransmittal(new TransmittalViewModel());
            Assert.IsTrue(result.ViewName.Equals("CreateTransmital"));
        }

        //[TestMethod]
        public void Verify_SendingTransmittalEmail()
        {
            AsyncMailController asyncMailController = new AsyncMailController();
            Transmittal transmittal = new Transmittal()
                {
                    CreatedBy = new User() {FirstName = "Ketul", LastName = "Patel"},
                    TransmittalStatus = new Status() {StatusText = "Approver"},
                    Subject = "Review docs",
                    TransmittalNumber = "Trans-123",
                    ProjectName = "Bishops Ave",
                    CustomerName = "Harrison Varma",
                    Distribution =
                        new List<TransmittalUser>()
                            {
                                new TransmittalUser()
                                    {
                                        Email = "pateketu@gmail.com",
                                        FirstName = "Ketul",
                                        LastName = "Patel",
                                        Company = new Company() {CompanyName = "Harrison Varma"}
                                    },
                                    new TransmittalUser()
                                    {
                                        Email = "Test@Test.com",
                                        FirstName = "Test",
                                        LastName = "Patel",
                                        Company = new Company() {CompanyName = "Harrison Varma"},
                                        IsCced = true
                                    }

                            }

                };
            //asyncMailController.TransmittalEmail(transmittal, "C:\\LocalBlobStorage\\IssueSheets\\IssueSheet_2001.pdf").Deliver();


        }

    }
}
