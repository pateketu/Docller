using System.Threading;
using System.Web;

namespace Docller.Core.Common
{
    public interface IClientConnection
    {
        bool IsClientConnected { get; }
        void TransmitFile(string fileName);
        CancellationToken ClientCancellationToken { get; }
    }

    class NullClientConnection : IClientConnection
    {
        public bool IsClientConnected { get { return true; } }
        public void TransmitFile(string fileName)
        {
            //
        }

        public CancellationToken ClientCancellationToken { get { return new CancellationToken(); } }
    }

    public  class ClientConnection : IClientConnection
    {
        private readonly HttpResponseBase _response;

        public ClientConnection(HttpResponseBase response)
        {
            _response = response;
        }

        public bool IsClientConnected
        {
            get { return _response.IsClientConnected; }
        }

        public void TransmitFile(string fileName)
        {
            _response.TransmitFile(fileName);
        }

        public CancellationToken ClientCancellationToken
        {
            get { return this._response.ClientDisconnectedToken; }
        }
    }
}