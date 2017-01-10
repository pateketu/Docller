using System.Threading.Tasks;
using System.Web;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.UI.Common
{
	public class TransmittalCreationActions
	{
	    private readonly IIssueSheetProvider _issueSheetProvider;
        private class ThreadState
        {
            public TransmittalCreationInfo Transmittal;
            public HttpContextBase Context;
        }
	    public TransmittalCreationActions( IIssueSheetProvider issueSheetProvider)
	    {
	        _issueSheetProvider = issueSheetProvider;
	    }

	    public virtual void Start(HttpContextBase context, TransmittalCreationInfo transmittalInfo)
        {
            ThreadState state = new ThreadState() {Transmittal = transmittalInfo,Context = context};

            Task.Factory.StartNew(AsyncStart, state, TaskCreationOptions.DenyChildAttach);
            //AsyncStart(state);
        }

        protected virtual void AsyncStart(object state)
        {
            ThreadState threadState = (ThreadState)state;

            string generatedIssueSheetFile = _issueSheetProvider.Create(threadState.Transmittal.IssueSheetData);

            Notify(threadState.Context, threadState.Transmittal.Transmittal, generatedIssueSheetFile);
        }   

        protected virtual void Notify(HttpContextBase context, Transmittal transmittal, string issueSheetFile)
        {

            //string template = "Hello @Model.Name, welcome to RazorEngine!";
            //string result = Razor.Parse(template, new { Name = "World" });

            //MailController mailController = new MailController() {HttpContextBase = context};
            //mailController.TransmittalEmail(transmittal,issueSheetFile).Deliver();
            //new MailController(){ }    TransmittalEmail(transmittal,issueSheetFile).Deliver();}
            AsyncMailController asyncMailController = new AsyncMailController {HttpContextBase = context};

            asyncMailController.TransmittalEmail(
                new TransmittalEmailView(transmittal)
                    {
                        RootUrl =Utils.GetRootUrl(context)
                    },
                issueSheetFile).Deliver();
        }
	}

   

    
}